namespace KidsEnglish.Core
{
    /// <summary>
    /// Unit 1 locked content keys. Spellings are exact — do not alias (no hello/goodbye/thanks).
    /// Addressables addresses match folders: Audio/{key} and Art/{key}.
    /// </summary>
    public static class ContentKeys
    {
        public const string Hi = "hi";
        public const string Bye = "bye";
        public const string Friend = "friend";
        public const string Me = "me";
        public const string You = "you";
        public const string Happy = "happy";
        public const string Sad = "sad";
        public const string Play = "play";
        public const string Name = "name";
        public const string Come = "come";

        public const string HiBye = "hi_bye";
        public const string ComePlay = "come_play";

        /// <summary>
        /// Full Unit 1 catalog (later levels). HelloFriends v1 does not teach these as a grid.
        /// </summary>
        public static readonly string[] Vocab =
        {
            Hi, Bye, Friend, Me, You, Happy, Sad, Play, Name, Come
        };

        public static readonly string[] Patterns =
        {
            HiBye, ComePlay
        };

        /// <summary>
        /// OnPlayDemo clipIds for L1 (not file names). Wrap-up uses
        /// bye_required_bunny / bye_required_fox — see L1HelloFriends.
        /// Scene VO files are vo_who_says_hi, vo_come_play_hint, vo_hi_speak, vo_bye_expose.
        /// </summary>
        public static readonly string[] L1V1Audio =
        {
            Hi, ComePlay, Bye
        };

        public const string SfxSuccess = "sfx_success";
        public const string SfxRetry = "sfx_retry";
        public const string SfxStar = "sfx_star";
        public const string SfxWrongSoft = "sfx_wrong_soft";

        public const string VoWhoSaysHi = "vo_who_says_hi";
        public const string VoComePlayHint = "vo_come_play_hint";
        public const string VoHiSpeak = "vo_hi_speak";
        public const string VoByeExpose = "vo_bye_expose";

        public const string LevelHelloFriends = "L1_HelloFriends";

        public static readonly string[] ParkGateArt =
        {
            "bg_park_gate", "char_bunny", "char_fox", "prop_tree", "prop_rock", "prop_ball", "drop_fox"
        };

        /// <summary>Addressable address and Resources path for a vocab/pattern clip.</summary>
        public static string Audio(string key) => "Audio/" + key;

        /// <summary>Addressable address and Resources path for a vocab/pattern card.</summary>
        public static string Art(string key) => "Art/" + key;

        public static string AudioAssetPath(string key) => "Assets/Audio/" + key + ".wav";

        public static string ArtAssetPath(string key) => "Assets/Art/" + key + ".png";

        public static string SfxAssetPath(string sfxKey) => "Assets/Audio/Sfx/" + sfxKey + ".wav";
    }
}
