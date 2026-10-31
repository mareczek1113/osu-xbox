// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#if XBOX
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using osu.Framework.Audio.Mixing;
using osu.Framework.Bindables;
using osu.Framework.Logging;
using osu.Framework.Statistics;
using SampleBase = osu.Framework.Audio.Sample.Sample;
using SampleChannelBase = osu.Framework.Audio.Sample.SampleChannel;

namespace osu.Framework.Audio.SDL
{
    /// <summary>
    /// Decodes a sample once and hands out <see cref="SampleSDL"/> instances sharing the decoded data.
    /// </summary>
    internal sealed class SampleSDLFactory : AudioCollectionManager<AdjustableAudioComponent>
    {
        public string Name { get; }

        public DecodedAudio? Audio { get; private set; }

        public override bool IsLoaded => Audio != null;

        public double Length { get; private set; }

        internal readonly Bindable<int> PlaybackConcurrency = new Bindable<int>(SampleBase.DEFAULT_CONCURRENCY);

        private readonly SDLAudioMixer mixer;
        private byte[]? data;

        public SampleSDLFactory(byte[] data, string name, SDLAudioMixer mixer)
        {
            this.data = data;
            this.mixer = mixer;
            Name = name;

            EnqueueAction(load);
        }

        private void load()
        {
            if (data == null || Audio != null)
                return;

            try
            {
                Audio = AudioDecoder.Decode(data);
                Length = Audio.LengthMs;
            }
            catch (InvalidDataException e)
            {
                Logger.Log($"Failed to load sample '{Name}': {e.Message}", LoggingTarget.Runtime, LogLevel.Important);
            }

            data = null;
        }

        public SampleBase CreateSample() => new SampleSDL(this, mixer) { OnPlay = onPlay };

        private void onPlay(SampleBase sample) => AddItem(sample);
    }

    internal sealed class SampleSDL : SampleBase
    {
        private readonly SampleSDLFactory factory;
        private readonly SDLAudioMixer mixer;
        private readonly List<SampleChannelSDL> started = new List<SampleChannelSDL>();

        public DecodedAudio? Audio => factory.Audio;


        public override bool IsLoaded => factory.IsLoaded;

        public override double Length => factory.Length;

        internal SampleSDL(SampleSDLFactory factory, SDLAudioMixer mixer)
            : base(factory.Name)
        {
            this.factory = factory;
            this.mixer = mixer;

            PlaybackConcurrency.BindTo(factory.PlaybackConcurrency);
        }

        protected override SampleChannelBase CreateChannel()
        {
            var channel = new SampleChannelSDL(this);
            mixer.Add(channel);
            return channel;
        }

        /// <summary>
        /// Enforces <see cref="SampleBase.PlaybackConcurrency"/>: when too many channels of this sample are playing, the longest-playing ones are stopped.
        /// </summary>
        internal void OnChannelStarted(SampleChannelSDL channel)
        {
            lock (started)
            {
                started.RemoveAll(c => c != channel && !c.Playing);

                if (!started.Contains(channel))
                    started.Add(channel);

                while (started.Count > Math.Max(1, PlaybackConcurrency.Value))
                {
                    started[0].StopForConcurrency();
                    started.RemoveAt(0);
                }
            }
        }
    }

    internal sealed class SampleChannelSDL : SampleChannelBase, ISDLAudioChannel
    {
        private readonly SampleSDL sample;

        // Written by the audio component thread, read by the device thread.
        private volatile bool playing;
        private volatile bool userRequestedPlay;
        private int pendingStart;
        private volatile bool looping;

        private double volume = 1;
        private double balance;
        private double frequency = 1;

        // Only touched by the device thread.
        private double position;

        // Latency telemetry: Stopwatch timestamps of the last Play() request and of when the device thread first mixed it.
        internal long PlayRequestedTimestamp;
        internal long FirstMixedTimestamp;

        public SampleChannelSDL(SampleSDL sample)
            : base(sample.Name)
        {
            this.sample = sample;
        }

        public override bool Playing => playing || Volatile.Read(ref pendingStart) == 1 || (Looping && userRequestedPlay);

        public override bool Looping
        {
            get => base.Looping;
            set
            {
                base.Looping = value;
                looping = value;
            }
        }

        public override void Play()
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);

            userRequestedPlay = true;
            Volatile.Write(ref FirstMixedTimestamp, 0);
            Volatile.Write(ref PlayRequestedTimestamp, System.Diagnostics.Stopwatch.GetTimestamp());
            Volatile.Write(ref pendingStart, 1);

            sample.OnChannelStarted(this);
            base.Play();

            InvalidateState();
        }

        public override void Stop()
        {
            userRequestedPlay = false;
            Volatile.Write(ref pendingStart, 0);
            playing = false;

            base.Stop();
        }

        internal void StopForConcurrency()
        {
            Volatile.Write(ref pendingStart, 0);
            playing = false;
        }

        internal override void OnStateChanged()
        {
            base.OnStateChanged();

            Volatile.Write(ref volume, AggregateVolume.Value);
            Volatile.Write(ref balance, AggregateBalance.Value);
            Volatile.Write(ref frequency, AggregateFrequency.Value);
        }

        protected override void UpdateState()
        {
            FrameStatistics.Increment(StatisticsCounterType.SChannels);
            base.UpdateState();
        }

        public void Mix(Span<float> dst, int frames)
        {
            if (Interlocked.Exchange(ref pendingStart, 0) == 1)
            {
                position = 0;
                playing = true;
                Volatile.Write(ref FirstMixedTimestamp, System.Diagnostics.Stopwatch.GetTimestamp());
            }

            if (!playing)
                return;

            var audio = sample.Audio;

            if (audio == null)
            {
                playing = false;
                return;
            }

            float[] src = audio.Samples;
            int channels = audio.Channels;
            int length = audio.Frames;

            double vol = Volatile.Read(ref volume);
            double pan = Volatile.Read(ref balance);
            double step = (double)audio.SampleRate / SDLAudioOutput.OUTPUT_RATE * Volatile.Read(ref frequency);

            float gainL = (float)(vol * (pan > 0 ? 1 - pan : 1));
            float gainR = (float)(vol * (pan < 0 ? 1 + pan : 1));

            bool loop = looping;
            double p = position;

            for (int i = 0; i < frames; i++)
            {
                if (p >= length)
                {
                    if (!loop)
                    {
                        playing = false;
                        break;
                    }

                    p -= length;
                }

                int i0 = (int)p;
                int i1 = i0 + 1 < length ? i0 + 1 : (loop ? 0 : i0);
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

            position = p;
        }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            playing = false;
            Volatile.Write(ref pendingStart, 0);
            (Mixer as SDLAudioMixer)?.ChannelFree(this);

            base.Dispose(disposing);
        }
    }
}

#endif