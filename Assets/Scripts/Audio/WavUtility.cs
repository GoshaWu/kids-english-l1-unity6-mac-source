using System;
using System.IO;
using UnityEngine;

namespace KidsEnglish.Audio
{
    /// <summary>
    /// Minimal 16-bit PCM WAV writer. Local files only.
    /// </summary>
    public static class WavUtility
    {
        public static void WriteClip(string path, AudioClip clip)
        {
            if (clip == null)
                throw new ArgumentNullException(nameof(clip));

            var samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);
            WritePcm16(path, samples, clip.channels, clip.frequency);
        }

        public static void WriteSilent(string path, float seconds, int frequency = 16000)
        {
            var count = Mathf.Max(1, Mathf.RoundToInt(seconds * frequency));
            WritePcm16(path, new float[count], 1, frequency);
        }

        public static AudioClip MakeTone(string name, float[] frequencies, float seconds, int sampleRate = 22050)
        {
            var len = Mathf.Max(1, Mathf.RoundToInt(seconds * sampleRate));
            var data = new float[len];
            var step = len / Mathf.Max(1, frequencies.Length);
            for (var i = 0; i < len; i++)
            {
                var f = frequencies[Mathf.Min(frequencies.Length - 1, i / step)];
                var env = Mathf.Sin(Mathf.PI * i / len);
                data[i] = env * 0.28f * Mathf.Sin(2f * Mathf.PI * f * i / sampleRate);
            }

            var clip = AudioClip.Create(name, len, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        static void WritePcm16(string path, float[] samples, int channels, int frequency)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(stream))
            {
                const short bits = 16;
                var byteRate = frequency * channels * bits / 8;
                var blockAlign = (short)(channels * bits / 8);
                var dataLen = samples.Length * 2;

                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(36 + dataLen);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
                writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)channels);
                writer.Write(frequency);
                writer.Write(byteRate);
                writer.Write(blockAlign);
                writer.Write(bits);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
                writer.Write(dataLen);

                for (var i = 0; i < samples.Length; i++)
                {
                    var clamped = Mathf.Clamp(samples[i], -1f, 1f);
                    writer.Write((short)Mathf.RoundToInt(clamped * short.MaxValue));
                }
            }
        }
    }
}
