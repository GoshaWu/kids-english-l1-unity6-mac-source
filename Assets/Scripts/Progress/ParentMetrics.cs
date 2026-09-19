namespace KidsEnglish.Progress
{
    /// <summary>
    /// Parent-mode copy keys and light ids. Report 会指认 and 敢跟读 only —
    /// no accuracy rates, rankings, or red X.
    /// </summary>
    public static class ParentMetrics
    {
        public const string IdentifyTitleKey = "parent.identify.title";
        public const string IdentifyHiKey = "parent.identify.hi";
        public const string IdentifyFriendKey = "parent.identify.friend";
        public const string IdentifyMeKey = "parent.identify.me";
        public const string IdentifyYouKey = "parent.identify.you";
        public const string IdentifyHappyKey = "parent.identify.happy";
        public const string IdentifySadKey = "parent.identify.sad";
        public const string IdentifyPlayComeKey = "parent.identify.play_come";
        public const string IdentifyByeWaveKey = "parent.identify.bye_wave";

        public const string SpeakTitleKey = "parent.speak.title";
        public const string SpeakHiKey = "parent.speak.hi";
        public const string SpeakByeKey = "parent.speak.bye";
        public const string SpeakComePlayKey = "parent.speak.come_play";

        public const string IdentifyHi = "hi";
        public const string IdentifyFriend = "friend";
        public const string IdentifyMe = "me";
        public const string IdentifyYou = "you";
        public const string IdentifyHappy = "happy";
        public const string IdentifySad = "sad";
        public const string IdentifyPlayCome = "play_come";
        public const string IdentifyByeWave = "bye_wave";

        public const string SpeakHi = "hi";
        public const string SpeakBye = "bye";
        public const string SpeakComePlay = "come_play";

        public static readonly string[] IdentifyBaseIds =
        {
            IdentifyHi, IdentifyFriend, IdentifyMe, IdentifyYou, IdentifyHappy, IdentifySad
        };

        public static readonly string[] IdentifyBonusIds =
        {
            IdentifyPlayCome, IdentifyByeWave
        };

        public static readonly string[] SpeakIds =
        {
            SpeakHi, SpeakBye, SpeakComePlay
        };

        public static string Glyph(bool on)
        {
            return on ? "●" : "○";
        }
    }
}
