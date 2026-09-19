using UnityEngine;

namespace KidsEnglish.Audio
{
    /// <summary>
    /// Default scorer: always returns a gentle success. No network, no vendor SDK.
    /// Swap this type in AppContext when a real on-device model exists.
    /// </summary>
    public sealed class StubSpeechScorer : ISpeechScorer
    {
        public SpeechScoreResult Score(string audioPath, float durationSeconds)
        {
            Debug.Log($"[KidsEnglish] StubSpeechScorer path={audioPath} dur={durationSeconds:0.00} (not uploaded)");
            return SpeechScoreResult.StubSuccess(durationSeconds);
        }
    }
}
