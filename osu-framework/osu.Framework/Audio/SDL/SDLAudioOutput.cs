// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#if XBOX
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using osu.Framework.Logging;
using SDL;
using static SDL.SDL3;

namespace osu.Framework.Audio.SDL
{
    /// <summary>
    /// Owns the single SDL3 audio output stream. Its callback pulls audio from every registered <see cref="SDLAudioMixer"/>,
    /// sums it, clips it and hands it to the device. Replaces BASS' device + global mixer.
    /// </summary>
    internal static unsafe class SDLAudioOutput
    {
        public const int OUTPUT_RATE = 48000;
        public const int CHANNELS = 2;

        private const int bytes_per_frame = sizeof(float) * CHANNELS;
        private const int max_frames_per_callback = 8192;

        private static readonly object gate = new object();
        private static readonly float[] mix_buffer = new float[max_frames_per_callback * CHANNELS];

        private static SDL_AudioStream* stream;
        private static volatile SDLAudioMixer[] mixers = Array.Empty<SDLAudioMixer>();

        private static long framesRendered;
        private static int peakBits;

        public static bool IsInitialised => stream != null;

        /// <summary>Frames the device requests per period (the output buffer size).</summary>
        public static int DeviceBufferFrames { get; private set; }

        /// <summary>Total frames handed to the device since initialisation.</summary>
        public static long FramesRendered => Interlocked.Read(ref framesRendered);

        /// <summary>Returns and resets the highest absolute sample level (post-clip) rendered since the last call.</summary>
        public static float TakePeak() => BitConverter.Int32BitsToSingle(Interlocked.Exchange(ref peakBits, 0));

        public static bool Init()
        {
            lock (gate)
            {
                if (stream != null)
                    return true;

                if (!SDL_InitSubSystem(SDL_InitFlags.SDL_INIT_AUDIO))
                {
                    Logger.Log($"SDL audio failed to initialise: {SDL_GetError()}", LoggingTarget.Runtime, LogLevel.Error);
                    return false;
                }

                var spec = new SDL_AudioSpec { format = SDL_AudioFormat.SDL_AUDIO_F32LE, channels = CHANNELS, freq = OUTPUT_RATE };
                stream = SDL_OpenAudioDeviceStream(SDL_AUDIO_DEVICE_DEFAULT_PLAYBACK, &spec, (delegate* unmanaged[Cdecl]<nint, SDL_AudioStream*, int, int, void>)&callback, 0);

                if (stream == null)
                {
                    Logger.Log($"SDL audio failed to open an output device: {SDL_GetError()}", LoggingTarget.Runtime, LogLevel.Error);
                    SDL_QuitSubSystem(SDL_InitFlags.SDL_INIT_AUDIO);
                    return false;
                }

                SDL_AudioSpec deviceSpec;
                int frames;
                SDL_GetAudioDeviceFormat(SDL_GetAudioStreamDevice(stream), &deviceSpec, &frames);
                DeviceBufferFrames = frames;

                SDL_ResumeAudioStreamDevice(stream);

                Logger.Log($"SDL audio initialised (driver {SDL_GetCurrentAudioDriver()}, {deviceSpec.freq} Hz, device buffer {frames} frames = {1000.0 * frames / deviceSpec.freq:F1} ms)");
                return true;
            }
        }

        public static void Shutdown()
        {
            lock (gate)
            {
                if (stream == null)
                    return;

                SDL_DestroyAudioStream(stream);
                stream = null;
                SDL_QuitSubSystem(SDL_InitFlags.SDL_INIT_AUDIO);
            }
        }

        public static void Register(SDLAudioMixer mixer)
        {
            lock (gate)
            {
                var next = new SDLAudioMixer[mixers.Length + 1];
                Array.Copy(mixers, next, mixers.Length);
                next[^1] = mixer;
                mixers = next;
            }
        }

        public static void Unregister(SDLAudioMixer mixer)
        {
            lock (gate)
                mixers = Array.FindAll(mixers, m => m != mixer);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        private static void callback(nint userdata, SDL_AudioStream* s, int additional, int total)
        {
            // Exceptions must never propagate into native code.
            try
            {
                int frames = Math.Min(additional / bytes_per_frame, max_frames_per_callback);
                if (frames <= 0)
                    return;

                Span<float> buffer = mix_buffer.AsSpan(0, frames * CHANNELS);
                buffer.Clear();

                var current = mixers;
                for (int i = 0; i < current.Length; i++)
                    current[i].Render(buffer, frames);

                float peak = 0;

                for (int i = 0; i < buffer.Length; i++)
                {
                    float v = buffer[i];
                    if (v > 1f) v = 1f;
                    else if (v < -1f) v = -1f;
                    buffer[i] = v;

                    float a = Math.Abs(v);
                    if (a > peak) peak = a;
                }

                if (peak > BitConverter.Int32BitsToSingle(Volatile.Read(ref peakBits)))
                    Volatile.Write(ref peakBits, BitConverter.SingleToInt32Bits(peak));

                fixed (float* p = mix_buffer)
                    SDL_PutAudioStreamData(s, (IntPtr)p, frames * bytes_per_frame);

                Interlocked.Add(ref framesRendered, frames);
            }
            catch
            {
            }
        }
    }
}

#endif