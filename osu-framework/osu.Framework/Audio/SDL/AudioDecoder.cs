// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#if XBOX

using System;
using System.IO;
using System.Text;

namespace osu.Framework.Audio.SDL
{
    /// <summary>
    /// Fully decoded PCM audio, interleaved 32-bit float.
    /// </summary>
    internal sealed class DecodedAudio
    {
        public float[] Samples { get; }
        public int Channels { get; }
        public int SampleRate { get; }

        public int Frames => Samples.Length / Channels;
        public double LengthMs => 1000.0 * Frames / SampleRate;

        public DecodedAudio(float[] samples, int channels, int sampleRate)
        {
            if (channels < 1) throw new InvalidDataException("Audio has no channels.");
            if (sampleRate < 1) throw new InvalidDataException("Audio has an invalid sample rate.");

            Samples = samples;
            Channels = channels;
            SampleRate = sampleRate;
        }
    }

    /// <summary>
    /// Managed audio decoding (WAV, Ogg Vorbis, MP3) with no native dependencies, for platforms where BASS is unavailable.
    /// </summary>
    internal static class AudioDecoder
    {
        /// <exception cref="InvalidDataException">The data is not a supported or valid audio file.</exception>
        public static DecodedAudio Decode(byte[] data)
        {
            if (data == null || data.Length < 4)
                throw new InvalidDataException("Audio data is empty.");

            try
            {
                if (hasTag(data, 0, "RIFF")) return decodeWav(data);
                if (hasTag(data, 0, "OggS")) return decodeVorbis(data);

                return decodeMp3(data);
            }
            catch (InvalidDataException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new InvalidDataException($"Failed to decode audio: {e.Message}", e);
            }
        }

        private static bool hasTag(byte[] d, int offset, string tag)
        {
            if (offset + tag.Length > d.Length) return false;

            for (int i = 0; i < tag.Length; i++)
            {
                if (d[offset + i] != tag[i]) return false;
            }

            return true;
        }

        private static DecodedAudio decodeWav(byte[] d)
        {
            if (d.Length < 12 || !hasTag(d, 8, "WAVE"))
                throw new InvalidDataException("Not a WAVE file.");

            int wavTag = 0, channels = 0, rate = 0, bits = 0;
            int dataOffset = -1, dataLength = 0;

            int pos = 12;

            while (pos + 8 <= d.Length)
            {
                string id = Encoding.ASCII.GetString(d, pos, 4);
                long size = BitConverter.ToUInt32(d, pos + 4);
                int body = pos + 8;

                if (id == "fmt ")
                {
                    if (body + 16 > d.Length) throw new InvalidDataException("Truncated fmt chunk.");

                    wavTag = BitConverter.ToUInt16(d, body);
                    channels = BitConverter.ToUInt16(d, body + 2);
                    rate = BitConverter.ToInt32(d, body + 4);
                    bits = BitConverter.ToUInt16(d, body + 14);

                    // WAVE_FORMAT_EXTENSIBLE: the real format tag is the first two bytes of the sub-format GUID.
                    if (wavTag == 0xFFFE && size >= 40 && body + 26 <= d.Length)
                        wavTag = BitConverter.ToUInt16(d, body + 24);
                }
                else if (id == "data")
                {
                    dataOffset = body;
                    // be lenient with truncated files and streaming-style sizes (0 / 0xFFFFFFFF).
                    dataLength = (int)Math.Min(size == 0 ? long.MaxValue : size, d.Length - body);
                    break;
                }

                long next = body + size + (size & 1);
                if (next > d.Length) break;

                pos = (int)next;
            }

            if (wavTag == 0 || dataOffset < 0) throw new InvalidDataException("WAVE file is missing its fmt or data chunk.");
            if (channels < 1 || rate < 1 || bits % 8 != 0 || bits == 0) throw new InvalidDataException("WAVE file has an invalid format.");

            bool isFloat = wavTag == 3;
            if (wavTag != 1 && !isFloat) throw new InvalidDataException($"Unsupported WAVE format tag {wavTag}.");
            if (isFloat && bits != 32) throw new InvalidDataException("Unsupported float WAVE bit depth.");
            if (bits > 32) throw new InvalidDataException("Unsupported WAVE bit depth.");

            int bytesPerSample = bits / 8;
            int frames = dataLength / (bytesPerSample * channels);
            var samples = new float[frames * channels];

            for (int i = 0; i < samples.Length; i++)
            {
                int o = dataOffset + i * bytesPerSample;

                samples[i] = bits switch
                {
                    8 => (d[o] - 128) / 128f,
                    16 => BitConverter.ToInt16(d, o) / 32768f,
                    24 => ((d[o] << 8 | d[o + 1] << 16 | d[o + 2] << 24) >> 8) / 8388608f,
                    32 when isFloat => BitConverter.ToSingle(d, o),
                    32 => BitConverter.ToInt32(d, o) / 2147483648f,
                    _ => throw new InvalidDataException("Unsupported WAVE bit depth.")
                };
            }

            return new DecodedAudio(samples, channels, rate);
        }

        private static DecodedAudio decodeVorbis(byte[] d)
        {
            using var reader = new NVorbis.VorbisReader(new MemoryStream(d), true);
            return readAll(reader.Channels, reader.SampleRate, reader.TotalSamples, (buf, count) => reader.ReadSamples(buf, 0, count));
        }

        private static DecodedAudio decodeMp3(byte[] d)
        {
            using var file = new NLayer.MpegFile(new MemoryStream(d));
            long totalFrames = file.Length > 0 ? file.Length / (sizeof(float) * file.Channels) : 0;
            var audio = readAll(file.Channels, file.SampleRate, totalFrames, (buf, count) => file.ReadSamples(buf, 0, count));
            return trimMp3Padding(audio, d);
        }

        /// <summary>
        /// MP3 encoders and decoders add silence at both ends. Players that honour the LAME/Xing tag remove it, which keeps
        /// audio aligned with the timing points authored against it. Without this the decode is late by ~1105 frames (~25 ms at 44.1 kHz).
        /// </summary>
        private const int mp3_decoder_delay = 529;

        internal static bool TryGetMp3Gapless(byte[] d, out int encoderDelay, out int encoderPadding)
        {
            encoderDelay = encoderPadding = 0;
            int limit = Math.Min(d.Length, 65536);

            int x = indexOf(d, "Xing", 0, limit);
            if (x < 0) x = indexOf(d, "Info", 0, limit);
            if (x < 0) return false;

            int e = -1;

            foreach (string id in new[] { "LAME", "Lavf", "Lavc" })
            {
                int i = indexOf(d, id, x, Math.Min(limit, x + 200));
                if (i >= 0 && (e < 0 || i < e)) e = i;
            }

            if (e < 0 || e + 24 > d.Length) return false;

            encoderDelay = (d[e + 21] << 4) | (d[e + 22] >> 4);
            encoderPadding = ((d[e + 22] & 0xF) << 8) | d[e + 23];
            return encoderDelay < 4096 && encoderPadding < 4096;
        }

        private static int indexOf(byte[] d, string tag, int from, int to)
        {
            for (int i = from; i + tag.Length <= to; i++)
            {
                if (hasTag(d, i, tag)) return i;
            }

            return -1;
        }

        private static DecodedAudio trimMp3Padding(DecodedAudio audio, byte[] source)
        {
            if (!TryGetMp3Gapless(source, out int delay, out int padding))
                return audio;

            int skip = delay + mp3_decoder_delay;
            int tail = Math.Max(0, padding - mp3_decoder_delay);

            if (skip + tail >= audio.Frames)
                return audio;

            float[] trimmed = new float[(audio.Frames - skip - tail) * audio.Channels];
            Array.Copy(audio.Samples, skip * audio.Channels, trimmed, 0, trimmed.Length);
            return new DecodedAudio(trimmed, audio.Channels, audio.SampleRate);
        }

        private static DecodedAudio readAll(int channels, int rate, long expectedFrames, Func<float[], int, int> read)
        {
            float[] output = new float[Math.Max(expectedFrames, 4096) * channels];
            int written = 0;
            float[] chunk = new float[4096 * channels];

            int n;

            while ((n = read(chunk, chunk.Length)) > 0)
            {
                if (written + n > output.Length)
                    Array.Resize(ref output, Math.Max(output.Length * 2, written + n));

                Array.Copy(chunk, 0, output, written, n);
                written += n;
            }

            if (written == 0) throw new InvalidDataException("Audio contains no samples.");

            Array.Resize(ref output, written - written % channels);
            return new DecodedAudio(output, channels, rate);
        }
    }
}

#endif