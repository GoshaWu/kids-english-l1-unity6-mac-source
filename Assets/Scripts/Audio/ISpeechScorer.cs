using System;

namespace KidsEnglish.Audio
{
    /// <summary>
    /// Pronunciation / listen-and-repeat scoring contract.
    /// Implementations must stay on-device. Do not upload raw voice.
    /// </summary>
    public interface ISpeechScorer
    {
        SpeechScoreResult Score(string audioPath, float durationSeconds);
    }

    [Serializable]
    public struct SpeechScoreResult
    {
        public float Score;
        public string Status;
        public string Message;

        public static SpeechScoreResult StubSuccess(float durationSeconds)
        {
            return new SpeechScoreResult
            {
                Score = 0.92f,
                Status = "stub_success",
                Message = durationSeconds > 0.15f
                    ? "Nice speaking! (local stub — no cloud ASR)"
                    : "Heard you! (local stub — no cloud ASR)"
            };
        }
    }
}
