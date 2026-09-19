using UnityEngine;

namespace KidsEnglish.Core
{
    /// <summary>
    /// Temporary art/UI tokens (wireframe lock). 1 canvas unit = 1 logical px at 390×844 portrait.
    /// </summary>
    public static class DesignTokens
    {
        public static readonly Color Bg = Hex("A8D8F0");
        public static readonly Color Primary = Hex("3B82F6");
        public static readonly Color Success = Hex("43A047");
        public static readonly Color SoftWarn = Hex("F59E0B");
        public static readonly Color Card = Hex("FFFFFF");
        public static readonly Color Text = Hex("1A1A1A");
        public static readonly Color TextMuted = Hex("334155");

        public const int FontMinSp = 20;
        public const int FontBodySp = 22;
        public const int FontTitleSp = 28;

        public const float ParentLockDp = 64f;
        public const float ListenDemoDp = 80f;
        public const float CardMinDp = 120f;
        public const float CardSpacingDp = 16f;
        public const float ParkHitDp = 80f;
        public const float ParkSpacingPx = 24f;
        public const float ParkSpacingDp = 24f;
        public const float BallSnapPx = 64f;
        public const float BallSnapDp = 64f;
        public const float MicDp = 96f;
        public const float HitSlopDp = 12f;
        public const float ScreenPadDp = 16f;

        public const float TapSeconds = 0.15f;
        public const float CelebrationSeconds = 1.15f;
        public const float SpeakListenMin = 5f;
        public const float SpeakListenMax = 8f;
        public const float SpeakProcessMax = 1.5f;

        public static readonly Vector2 PortraitReference = new Vector2(390f, 844f);

        public static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString("#" + hex.TrimStart('#'), out var c);
            return c;
        }
    }

    public enum ListenUiState
    {
        WaitingToListen = 0,
        Playing = 1,
        WaitingChoice = 2,
        Correct = 3,
        Wrong = 4
    }

    public enum SpeakUiState
    {
        Idle = 0,
        Listening = 1,
        Processing = 2,
        Success = 3,
        Retry = 4
    }
}
