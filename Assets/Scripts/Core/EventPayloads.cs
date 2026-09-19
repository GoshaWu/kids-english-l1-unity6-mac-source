using System;

namespace KidsEnglish.Core
{
    /// <summary>
    /// Payloads for the five gameplay events. Field names match the locked L1 script.
    /// Keep these small: no child identity, no raw audio bytes.
    /// LevelId is extra routing for the bus (one scene may host parent UI).
    /// </summary>
    [Serializable]
    public struct PlayDemoPayload
    {
        public string LevelId;
        public string ClipId;
        public string TargetId;
        public bool CanReplay;
        public bool Assess;
    }

    [Serializable]
    public struct ChoicePayload
    {
        public string LevelId;
        public string ChoiceId;
        public string OptionId;
        public bool Correct;
    }

    [Serializable]
    public struct RecordStartPayload
    {
        public string LevelId;
        public string UtteranceId;
    }

    [Serializable]
    public struct RecordStopPayload
    {
        public string LevelId;
        public string UtteranceId;
        public string AudioRef;
        public float Score;
    }

    public static class FeedbackKind
    {
        public const string Intro = "intro";
        public const string Demo = "demo";
        public const string Success = "success";
        public const string Retry = "retry";
        public const string Skip = "skip";
        public const string RecordStub = "record_stub";
        public const string Outro = "outro";
    }

    [Serializable]
    public struct FeedbackPayload
    {
        public string LevelId;
        public string Kind;
        public string TextKey;
        public int ScaffoldLevel;
        public string Message;
    }
}
