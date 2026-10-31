// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#if XBOX
using System;
using System.Linq;
using ManagedBass;
using osu.Framework.Audio.Mixing;
using osu.Framework.Logging;
using osu.Framework.Statistics;

namespace osu.Framework.Audio.SDL
{
    internal interface ISDLAudioChannel : IAudioChannel
    {
        /// <summary>Adds up to <paramref name="frames"/> frames of this channel (interleaved stereo, 48 kHz) into <paramref name="dst"/>. Called on the audio device thread.</summary>
        void Mix(Span<float> dst, int frames);
    }

    /// <summary>
    /// Software mixer for the SDL3 audio backend. Sums its channels into the shared output stream (see <see cref="SDLAudioOutput"/>).
    /// </summary>
    public class SDLAudioMixer : AudioMixer
    {
        private readonly object channelLock = new object();
        private volatile ISDLAudioChannel[] channels = Array.Empty<ISDLAudioChannel>();
        private bool warnedAboutEffects;

        public SDLAudioMixer(AudioMixer? fallbackMixer, string identifier)
            : base(fallbackMixer, identifier)
        {
            SDLAudioOutput.Register(this);
        }

        internal void Render(Span<float> dst, int frames)
        {
            var snapshot = channels;

            for (int i = 0; i < snapshot.Length; i++)
                snapshot[i].Mix(dst, frames);
        }

        protected override void AddInternal(IAudioChannel channel)
        {
            if (channel is not ISDLAudioChannel c)
                throw new InvalidOperationException($"{channel.GetType().Name} cannot be mixed by the SDL audio backend.");

            lock (channelLock)
            {
                if (!channels.Contains(c))
                    channels = channels.Append(c).ToArray();
            }
        }

        protected override void RemoveInternal(IAudioChannel channel)
        {
            lock (channelLock)
                channels = channels.Where(c => !ReferenceEquals(c, channel)).ToArray();
        }

        internal void ChannelFree(ISDLAudioChannel channel) => RemoveInternal(channel);

        // Effects (e.g. low-pass filters) are not implemented yet in this backend.
        public override void AddEffect(IEffectParameter effect, int priority = 0) => warnUnsupportedEffects();
        public override void RemoveEffect(IEffectParameter effect) => warnUnsupportedEffects();
        public override void UpdateEffect(IEffectParameter effect) => warnUnsupportedEffects();

        private void warnUnsupportedEffects()
        {
            if (warnedAboutEffects)
                return;

            warnedAboutEffects = true;
            Logger.Log($"Audio effects are not supported by the SDL audio backend yet (mixer '{Identifier}').", LoggingTarget.Runtime, LogLevel.Important);
        }

        protected override void UpdateState()
        {
            FrameStatistics.Add(StatisticsCounterType.MixChannels, channels.Length);
            base.UpdateState();
        }

        protected override void Dispose(bool disposing)
        {
            SDLAudioOutput.Unregister(this);
            base.Dispose(disposing);
        }
    }
}

#endif