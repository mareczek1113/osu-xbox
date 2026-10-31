// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#if XBOX
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Audio.Mixing;
using osu.Framework.Extensions;
using osu.Framework.Logging;
using TrackBase = osu.Framework.Audio.Track.Track;

namespace osu.Framework.Audio.SDL
{
    /// <summary>
    /// A fully decoded, in-memory track played through the SDL3 software mixer (the Xbox build's replacement for <c>TrackBass</c>).
    /// </summary>
    /// <remarks>
    /// Threading: the audio component thread owns the public state machine (start/stop/seek/complete). The device thread only
    /// reads the small set of volatile fields below from <see cref="Mix"/> and publishes the playback position back.
    /// </remarks>
    internal sealed class TrackSDL : TrackBase, ISDLAudioChannel
    {
        private DecodedAudio? audio;
        private volatile bool isLoaded;
        private int bitrate;

        // ---- audio thread -> device thread ----
        private volatile bool playing; // the device thread advances/mixes the track only while this is set.
        private volatile bool looping;
        private double volume = 1;
        private double balance;
        private double speed = 1; // signed source frames advanced per source-rate frame (frequency * tempo; negative = reversed).
        private double seekTarget; // in source frames.
        private int seekGeneration;

        // ---- device thread -> audio thread ----
        private double position; // in source frames. Only written by the device thread.
        private int positionGeneration; // the seek generation that <see cref="position"/> is valid for.
        private volatile bool ended;

        // ---- device thread only ----
        private int appliedSeekGeneration;

        // ---- audio thread only ----
        private volatile bool isRunning;
        private volatile bool hasCompleted;
        private double currentTime;

        public override bool IsLoaded => isLoaded;

        public override bool IsDummyDevice => false;

        public override int? Bitrate => bitrate;

        internal TrackSDL(Stream data, string name)
            : base(name)
        {
            ArgumentNullException.ThrowIfNull(data);

            byte[] bytes;

            try
            {
                bytes = readAll(data);
            }
            finally
            {
                data.Dispose();
            }

            try
            {
                // Decoding happens on the caller's thread (TrackStore.GetAsync runs on the thread pool) to keep the audio thread free.
                audio = AudioDecoder.Decode(bytes);
                Length = audio.LengthMs;
                bitrate = Length > 0 ? (int)Math.Round(bytes.Length * 8 / Length) : 0; // kbit/s, as reported by BASS.
                isLoaded = true;
            }
            catch (InvalidDataException e)
            {
                Logger.Log($"Failed to load track '{name}': {e.Message}", LoggingTarget.Runtime, LogLevel.Important);
            }

            InvalidateState();
        }

        private static byte[] readAll(Stream data)
        {
            if (data is MemoryStream ms)
                return ms.TryGetBuffer(out var seg) && seg.Offset == 0 && seg.Count == seg.Array!.Length ? seg.Array : ms.ToArray();

            using var copy = new MemoryStream();
            data.CopyTo(copy);
            return copy.ToArray();
        }

        #region Playback control (audio thread)

        public override void Start()
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            StartAsync().WaitSafely();
        }

        public override Task StartAsync() => EnqueueAction(() =>
        {
            // Matching BASS-backed behaviour: a completed track is not restarted implicitly. Seek back first.
            if (audio == null || hasCompleted)
                return;

            isRunning = true;
            playing = true;
        });

        public override void Stop() => StopAsync().WaitSafely();

        public override Task StopAsync() => EnqueueAction(() =>
        {
            playing = false;
            isRunning = false;
        });

        public override bool IsRunning => isRunning;

        public override bool HasCompleted => hasCompleted;

        public override bool Looping
        {
            get => base.Looping;
            set
            {
                base.Looping = value;
                looping = value;
            }
        }

        public override bool Seek(double seek) => SeekAsync(seek).GetResultSafely();

        public override async Task<bool> SeekAsync(double seek)
        {
            // The track may not be loaded yet (Length == 0); stay optimistic in that case, like TrackBass.
            double lastSeekable = audio == null ? 0 : 1000.0 * (audio.Frames - 1) / audio.SampleRate;
            double conservativeLength = Length == 0 ? double.MaxValue : lastSeekable;
            double conservativeClamped = Math.Clamp(seek, 0, conservativeLength);

            await EnqueueAction(() => seekInternal(seek)).ConfigureAwait(false);

            return conservativeClamped == seek;
        }

        private void seekInternal(double seek)
        {
            var a = audio;
            if (a == null)
                return;

            double clamped = Math.Clamp(seek, 0, Length);

            if (clamped < Length)
            {
                // A completed track stays stopped after seeking back (like BASS); the caller has to Start() it again.
                hasCompleted = false;
                ended = false;
            }

            // Order matters: the target must be visible before the generation changes.
            Volatile.Write(ref seekTarget, clamped / 1000.0 * a.SampleRate);
            Interlocked.Increment(ref seekGeneration);

            updateCurrentTime();
        }

        public override double CurrentTime => Volatile.Read(ref currentTime);

        private void updateCurrentTime()
        {
            var a = audio;
            if (a == null)
                return;

            // If the device thread hasn't applied the most recent seek yet, report where we are going to be, not where we were.
            int gen = Volatile.Read(ref positionGeneration);
            double frame = gen == Volatile.Read(ref seekGeneration) ? Volatile.Read(ref position) : Volatile.Read(ref seekTarget);

            Volatile.Write(ref currentTime, frame / a.SampleRate * 1000.0);
        }

        protected override void UpdateState()
        {
            base.UpdateState();

            if (ended)
            {
                ended = false;
                hasCompleted = true;
                isRunning = false;

                updateCurrentTime();
                RaiseCompleted();
                return;
            }

            updateCurrentTime();
        }

        internal override void OnStateChanged()
        {
            base.OnStateChanged();

            Volatile.Write(ref volume, AggregateVolume.Value);
            Volatile.Write(ref balance, AggregateBalance.Value);

            // TODO: tempo currently behaves like frequency (pitch follows speed). Pitch-preserving time stretch is still to do.
            double tempo = Math.Max(Math.Abs(AggregateTempo.Value), 0.05);
            Volatile.Write(ref speed, AggregateFrequency.Value * tempo);
        }

        #endregion

        #region Mixing (device thread)

        public void Mix(Span<float> dst, int frames)
        {
            var a = audio;
            if (a == null)
                return;

            double p = Volatile.Read(ref position);

            int gen = Volatile.Read(ref seekGeneration);

            if (gen != appliedSeekGeneration)
            {
                p = Volatile.Read(ref seekTarget);
                appliedSeekGeneration = gen;
            }

            double step = Volatile.Read(ref speed) * a.SampleRate / SDLAudioOutput.OUTPUT_RATE;

            if (playing && step != 0)
                p = render(a, dst, frames, p, step);

            // Publish the position first, then the generation it belongs to (readers check them in the opposite order).
            Volatile.Write(ref position, p);
            Volatile.Write(ref positionGeneration, appliedSeekGeneration);
        }

        private double render(DecodedAudio a, Span<float> dst, int frames, double p, double step)
        {
            float[] src = a.Samples;
            int channels = a.Channels;
            int length = a.Frames;

            double vol = Volatile.Read(ref volume);
            double pan = Volatile.Read(ref balance);

            float gainL = (float)(vol * (pan > 0 ? 1 - pan : 1));
            float gainR = (float)(vol * (pan < 0 ? 1 + pan : 1));

            bool loop = looping;
            double restartFrame = Math.Clamp(RestartPoint / 1000.0 * a.SampleRate, 0, Math.Max(0, length - 1));

            for (int i = 0; i < frames; i++)
            {
                if (step > 0 ? p >= length : p < 0)
                {
                    if (!loop)
                    {
                        p = step > 0 ? length : 0;
                        playing = false;
                        ended = true;
                        break;
                    }

                    p = step > 0 ? restartFrame + (p - length) : length + p;
                    p = Math.Clamp(p, 0, length - 1);
                }

                int i0 = (int)p;
                int i1 = i0 + 1 < length ? i0 + 1 : i0;
                float fr = (float)(p - i0);

                float l, r;

                if (channels == 1)
                {
                    l = r = src[i0] + (src[i1] - src[i0]) * fr;
                }
                else
                {
                    l = src[i0 * channels] + (src[i1 * channels] - src[i0 * channels]) * fr;
                    r = src[i0 * channels + 1] + (src[i1 * channels + 1] - src[i0 * channels + 1]) * fr;
                }

                dst[2 * i] += l * gainL;
                dst[2 * i + 1] += r * gainR;

                p += step;
            }

            return p;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            playing = false;
            isRunning = false;
            (Mixer as SDLAudioMixer)?.ChannelFree(this);

            base.Dispose(disposing);
        }
    }
}

#endif
