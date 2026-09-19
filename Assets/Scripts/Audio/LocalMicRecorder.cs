using System;
using System.IO;
using KidsEnglish.Core;
using UnityEngine;

namespace KidsEnglish.Audio
{
    /// <summary>
    /// Captures a short clip from the device mic and writes it under persistentDataPath.
    /// Privacy: files stay on device, filenames contain only level id + unix time (no child name).
    /// Never upload these bytes. Parent panel can delete the local folder.
    /// </summary>
    public sealed class LocalMicRecorder : MonoBehaviour
    {
        public const int SampleRate = 16000;
        public const int MaxSeconds = 8;

        public bool IsRecording { get; private set; }
        public string LastPath { get; private set; }
        public float LastDuration { get; private set; }

        AudioClip _clip;
        string _device;
        string _levelId;
        string _utteranceId;
        float _startedAt;

        public void StartCapture(string levelId, string utteranceId = "hi")
        {
            if (IsRecording)
                StopCapture();

            _levelId = levelId;
            _utteranceId = utteranceId;
            _device = Microphone.devices.Length > 0 ? Microphone.devices[0] : null;

            GameEventBus.RaiseRecordStart(new RecordStartPayload
            {
                LevelId = levelId,
                UtteranceId = utteranceId
            });

            if (!string.IsNullOrEmpty(_device))
            {
                _clip = Microphone.Start(_device, false, MaxSeconds, SampleRate);
            }
            else
            {
                Debug.LogWarning("[KidsEnglish] No microphone. Writing a local silent placeholder instead of capturing.");
                _clip = AudioClip.Create("mic_placeholder", SampleRate, 1, SampleRate, false);
            }

            _startedAt = Time.realtimeSinceStartup;
            IsRecording = true;
        }

        public string StopCapture()
        {
            if (!IsRecording)
                return LastPath;

            var duration = Mathf.Clamp(Time.realtimeSinceStartup - _startedAt, 0.05f, MaxSeconds);
            if (!string.IsNullOrEmpty(_device))
                Microphone.End(_device);

            var folder = Path.Combine(Application.persistentDataPath, "voice_local");
            Directory.CreateDirectory(folder);
            // No child name / account id in the filename.
            var fileName = $"{_levelId}_{_utteranceId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.wav";
            var path = Path.Combine(folder, fileName);

            try
            {
                if (_clip != null && _clip.samples > 0 && Microphone.devices.Length > 0)
                    WavUtility.WriteClip(path, _clip);
                else
                    WavUtility.WriteSilent(path, duration, SampleRate);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[KidsEnglish] Failed to write local wav: {ex.Message}");
                path = string.Empty;
            }

            LastPath = path;
            LastDuration = duration;
            IsRecording = false;
            _clip = null;

            GameEventBus.RaiseRecordStop(new RecordStopPayload
            {
                LevelId = _levelId,
                UtteranceId = _utteranceId,
                AudioRef = path,
                Score = duration >= 0.45f ? 0.92f : 0.35f
            });

            return path;
        }

        public static void DeleteLocalRecordings()
        {
            var folder = Path.Combine(Application.persistentDataPath, "voice_local");
            if (!Directory.Exists(folder))
                return;

            Directory.Delete(folder, true);
            Debug.Log("[KidsEnglish] Deleted local voice_local folder (parent-initiated).");
        }

        void OnDestroy()
        {
            if (IsRecording)
                StopCapture();
        }
    }
}
