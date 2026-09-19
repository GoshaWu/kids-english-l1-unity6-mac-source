using UnityEngine;

namespace KidsEnglish.Levels
{
    /// <summary>
    /// HARD LOCK — L1 v1 beats only. Ignore any gameplay v2 that expands this
    /// level to full-unit vocabulary (me / you / happy / sad / name, Hi/Bye grid,
    /// Happy/Sad, Play 2×2, Friend/Me/You). Timeline numbers are an authoring
    /// guide, not a wall-clock requirement.
    /// </summary>
    public enum L1Beat
    {
        Intro = 0,
        DemoHi = 1,
        ChoiceHi = 2,
        DemoCome = 3,
        TaskInvite = 4,
        OptionalRec = 5,
        ByeBunny = 6,
        ByeFox = 7,
        Outro = 8
    }

    /// <summary>
    /// Input phases. Phase 1 enables only Bunny/Tree/Rock. Phase 2 enables only
    /// hit_ball + drop_fox. Wrap-up has no taps and no scoring.
    /// </summary>
    public enum L1Phase
    {
        None = 0,
        ThreeChoice = 1,
        BallFox = 2,
        WrapUp = 3
    }

    public static class L1HelloFriends
    {
        public const string LevelId = "L1_HelloFriends";

        /// <summary>
        /// Vocab keys that must never appear as HelloFriends tasks or choice cards.
        /// Unit catalog may still reserve the files for later levels.
        /// </summary>
        public static readonly string[] ForbiddenInThisLevel =
        {
            "me", "you", "happy", "sad", "name"
        };

        /// <summary>Assessed choice optionIds on this level: Bunny/Tree/Rock then Fox.</summary>
        public static readonly string[] V1OptionIds =
        {
            Bunny, Tree, Rock, Fox
        };

        public const string Bunny = "bunny";
        public const string Tree = "tree";
        public const string Rock = "rock";
        public const string Fox = "fox";
        public const string Friend = "friend";
        public const string Ball = "ball";
        public const string HitBunny = "hit_bunny";
        public const string HitTree = "hit_tree";
        public const string HitRock = "hit_rock";
        public const string HitBall = "hit_ball";
        public const string DropFox = "drop_fox";

        public const string BgParkGate = "bg_park_gate";
        public const string CharBunny = "char_bunny";
        public const string CharFox = "char_fox";
        public const string PropTree = "prop_tree";
        public const string PropRock = "prop_rock";
        public const string PropBall = "prop_ball";

        public const string AnimBunnyWave = "anim_bunny_wave";
        public const string AnimWaveBunny = "anim_wave_bunny";
        public const string AnimFoxWave = "anim_fox_wave";
        public const string AnimFoxCatch = "anim_fox_catch";

        public const string UiBtnListen = "ui_btn_listen";
        public const string UiParentLock = "ui_parent_lock";
        public const string UiProgressDots = "progress_dots";

        public const string VoWhoSaysHi = "vo_who_says_hi";
        public const string VoComePlayHint = "vo_come_play_hint";
        public const string VoHiSpeak = "vo_hi_speak";
        public const string VoByeExpose = "vo_bye_expose";
        public const string SfxStar = "sfx_star";
        public const string SfxWrongSoft = "sfx_wrong_soft";

        public static readonly string[] L1Anims =
        {
            AnimBunnyWave, AnimWaveBunny, AnimFoxWave, AnimFoxCatch
        };

        public static readonly string[] L1Vo =
        {
            VoWhoSaysHi, VoComePlayHint, VoHiSpeak, VoByeExpose
        };

        public static readonly string[] L1Sfx =
        {
            SfxStar, SfxWrongSoft
        };

        public static readonly string[] ParkGateArt =
        {
            BgParkGate, CharBunny, CharFox, PropTree, PropRock, PropBall, DropFox
        };

        // Logical px on 390×844. Stage is 0.04–0.96 × 0.12–0.70 of the canvas.
        public static readonly Vector2 HitSizeBunny = new Vector2(96f, 140f);
        public static readonly Vector2 HitSizeTree = new Vector2(88f, 120f);
        public static readonly Vector2 HitSizeRock = new Vector2(88f, 88f);
        public static readonly Vector2 HitSizeBall = new Vector2(80f, 80f);
        public static readonly Vector2 HitSizeFox = new Vector2(100f, 120f);
        public static readonly Vector2 HitSizeFriend = new Vector2(88f, 120f);
        public const float SnapRadiusPx = 64f;
        public const float ChoiceSpacingPx = 24f;

        public static readonly Vector2 ParkBunny;
        public static readonly Vector2 ParkTree;
        public static readonly Vector2 ParkRock;
        public static readonly Vector2 ParkFox;
        public static readonly Vector2 ParkBall;
        public static readonly Vector2 ParkFriend;

        static L1HelloFriends()
        {
            const float canvasW = 390f;
            const float canvasH = 844f;
            const float stageW = canvasW * 0.92f;
            const float stageH = canvasH * 0.58f;
            const float gap = ChoiceSpacingPx;
            var row = HitSizeBunny.x + gap + HitSizeTree.x + gap + HitSizeRock.x;
            var side = (stageW - row) * 0.5f;
            const float choiceBottom = 220f;
            const float lowerBottom = 20f;
            const float pad = 16f;

            ParkBunny = new Vector2(
                (side + HitSizeBunny.x * 0.5f) / stageW,
                (choiceBottom + HitSizeBunny.y * 0.5f) / stageH);
            ParkTree = new Vector2(
                (side + HitSizeBunny.x + gap + HitSizeTree.x * 0.5f) / stageW,
                (choiceBottom + HitSizeTree.y * 0.5f) / stageH);
            ParkRock = new Vector2(
                (side + HitSizeBunny.x + gap + HitSizeTree.x + gap + HitSizeRock.x * 0.5f) / stageW,
                (choiceBottom + HitSizeRock.y * 0.5f) / stageH);
            ParkFox = new Vector2(
                (stageW - pad - HitSizeFox.x * 0.5f) / stageW,
                (lowerBottom + HitSizeFox.y * 0.5f) / stageH);
            ParkBall = new Vector2(
                (pad + HitSizeBall.x * 0.5f) / stageW,
                (lowerBottom + HitSizeBall.y * 0.5f) / stageH);
            ParkFriend = new Vector2(0.50f, (lowerBottom + HitSizeFriend.y * 0.5f) / stageH);
        }

        public static L1Phase PhaseOf(L1Beat beat)
        {
            switch (beat)
            {
                case L1Beat.ChoiceHi:
                    return L1Phase.ThreeChoice;
                case L1Beat.TaskInvite:
                    return L1Phase.BallFox;
                case L1Beat.ByeBunny:
                case L1Beat.ByeFox:
                case L1Beat.Outro:
                    return L1Phase.WrapUp;
                default:
                    return L1Phase.None;
            }
        }

        public const string ChoiceHi = "hi";
        public const string ChoiceComePlay = "come_play";

        public const string ClipHi = "hi";
        public const string ClipComePlay = "come_play";
        public const string ClipByeBunny = "bye_required_bunny";
        public const string ClipByeFox = "bye_required_fox";
        public const string UtteranceHi = "hi";

        public const int ScaffoldFull = 1;
        public const int ScaffoldReduced = 2;
        public const int ScaffoldWithdrawn = 3;

        public const int GuideIntro = 0;
        public const int GuideDemoHi = 10;
        public const int GuideChoiceHi = 25;
        public const int GuideDemoCome = 45;
        public const int GuideTaskInvite = 65;
        public const int GuideOptionalRec = 100;
        public const int GuideByeBunny = 110;
        public const int GuideByeFox = 115;
        public const int GuideOutro = 120;
    }
}
