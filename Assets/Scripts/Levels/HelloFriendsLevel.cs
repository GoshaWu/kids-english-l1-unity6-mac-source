using System.Collections;
using KidsEnglish.Audio;
using KidsEnglish.Core;
using KidsEnglish.KidInput;
using KidsEnglish.Parent;
using KidsEnglish.Progress;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace KidsEnglish.Levels
{
        /// <summary>
        /// HARD LOCK — L1 v1 park gate only:
        /// Who says Hi? (Bunny/Tree/Rock) → Come play! (ball → Fox) →
        /// optional Hi! (blockProgress=false) → Bye! twice
        /// (bye_required_bunny then bye_required_fox, assess=false) → done.
        /// Does not load me / you / happy / sad / name.
        /// </summary>
    public sealed class HelloFriendsLevel : LevelController
    {
        AudioPlayer _audio;
        ProgressStore _progress;
        LocalMicRecorder _recorder;
        ISpeechScorer _scorer;
        ParentModeController _parent;
        SpeakFeedbackPanel _speak;

        RectTransform _canvasRt;
        RectTransform _listenRt;
        Image _listenVisual;
        Text _listenCaption;
        Text _status;
        Text _star;
        CharacterHotspot _bunny;
        CharacterHotspot _tree;
        CharacterHotspot _rock;
        CharacterHotspot _fox;
        CharacterHotspot _friend;
        CharacterHotspot[] _choiceSpots;
        CharacterHotspot[] _hotspots;
        InviteBall _ball;
        Image _parkBg;

        AudioClip _hiClip;
        AudioClip _comeClip;
        AudioClip _byeClip;
        AudioClip _speakClip;
        AudioClip _successClip;
        AudioClip _retryClip;
        Image[] _progressDots;
        Sprite _bgSprite;
        Sprite _bunnySprite;
        Sprite _foxSprite;
        Sprite _treeSprite;
        Sprite _rockSprite;
        Sprite _ballSprite;
        Sprite _dropSprite;
        Sprite _friendSprite;
        Sprite _animBunnyWave;
        Sprite _animWaveBunny;
        Sprite _animFoxWave;
        Sprite _animFoxCatch;

        static readonly Color BunnyTint = new Color(0.97f, 0.73f, 0.82f);
        static readonly Color FoxTint = new Color(0.98f, 0.57f, 0.24f);

        L1Beat _beat = L1Beat.Intro;
        ListenUiState _listenState = ListenUiState.WaitingToListen;
        int _scaffold = L1HelloFriends.ScaffoldFull;
        bool _inviteCleared;
        bool _busy;
        int _inviteFails;
        bool _tapHintMode;
        Coroutine _pulseListen;
        Coroutine _demoRoutine;

        public L1Beat Beat => _beat;
        public L1Phase Phase => L1HelloFriends.PhaseOf(_beat);
        public int ScaffoldLevel => _scaffold;
        public ListenUiState ListenState => _listenState;

        void Awake()
        {
            LevelId = L1HelloFriends.LevelId;
            EnsureSceneShell();
            BuildUi();
        }

        IEnumerator Start()
        {
            yield return null;
            BindServices();
            yield return LoadContent();
            ApplyArt();
            EnterBeat(L1Beat.Intro);
        }

        void OnDestroy()
        {
            GameEventBus.OnRecordStop -= OnRecordStopSave;
            if (_speak != null)
            {
                _speak.Skipped -= OnSpeakFinished;
                _speak.Succeeded -= OnSpeakFinished;
            }
        }

        void BindServices()
        {
            if (AppContext.Instance == null)
            {
                var services = new GameObject("AppServices");
                services.AddComponent<AppContext>();
            }

            var ctx = AppContext.Instance;
            _audio = ctx.Audio;
            _progress = ctx.Progress;
            _recorder = ctx.Recorder;
            _scorer = ctx.Scorer;
            _parent = ctx.Parent;

            var parentView = ParentPanelView.Create(_canvasRt);
            _parent.Bind(_progress, parentView);

            _speak = SpeakFeedbackPanel.Create(_canvasRt);
            _speak.BlockProgress = false;
            _speak.Skipped += OnSpeakFinished;
            _speak.Succeeded += OnSpeakFinished;
            GameEventBus.OnRecordStop += OnRecordStopSave;
        }

        IEnumerator LoadContent()
        {
            yield return ContentLoader.LoadClip(ContentKeys.Audio(L1HelloFriends.VoWhoSaysHi), ContentKeys.Audio(L1HelloFriends.VoWhoSaysHi), c => _hiClip = c);
            if (_hiClip == null)
                yield return ContentLoader.LoadClip(ContentKeys.Audio(L1HelloFriends.ClipHi), ContentKeys.Audio(L1HelloFriends.ClipHi), c => _hiClip = c);
            yield return ContentLoader.LoadClip(ContentKeys.Audio(L1HelloFriends.VoComePlayHint), ContentKeys.Audio(L1HelloFriends.VoComePlayHint), c => _comeClip = c);
            if (_comeClip == null)
                yield return ContentLoader.LoadClip(ContentKeys.Audio(L1HelloFriends.ClipComePlay), ContentKeys.Audio(L1HelloFriends.ClipComePlay), c => _comeClip = c);
            yield return ContentLoader.LoadClip(ContentKeys.Audio(L1HelloFriends.VoByeExpose), ContentKeys.Audio(L1HelloFriends.VoByeExpose), c => _byeClip = c);
            if (_byeClip == null)
                yield return ContentLoader.LoadClip(ContentKeys.Audio(ContentKeys.Bye), ContentKeys.Audio(ContentKeys.Bye), c => _byeClip = c);
            yield return ContentLoader.LoadClip(ContentKeys.Audio(L1HelloFriends.VoHiSpeak), ContentKeys.Audio(L1HelloFriends.VoHiSpeak), c => _speakClip = c);
            yield return ContentLoader.LoadClip(ContentKeys.SfxStar, "Audio/" + ContentKeys.SfxStar, c => _successClip = c);
            if (_successClip == null)
                yield return ContentLoader.LoadClip(ContentKeys.SfxSuccess, "Audio/sfx_success", c => _successClip = c);
            yield return ContentLoader.LoadClip(ContentKeys.SfxWrongSoft, "Audio/" + ContentKeys.SfxWrongSoft, c => _retryClip = c);
            if (_retryClip == null)
                yield return ContentLoader.LoadClip(ContentKeys.SfxRetry, "Audio/sfx_retry", c => _retryClip = c);
            yield return ContentLoader.LoadSprite(ContentKeys.Art(L1HelloFriends.BgParkGate), ContentKeys.Art(L1HelloFriends.BgParkGate), s => _bgSprite = s);
            yield return ContentLoader.LoadSprite(ContentKeys.Art(L1HelloFriends.CharBunny), ContentKeys.Art(L1HelloFriends.CharBunny), s => _bunnySprite = s);
            yield return ContentLoader.LoadSprite(ContentKeys.Art(L1HelloFriends.CharFox), ContentKeys.Art(L1HelloFriends.CharFox), s => _foxSprite = s);
            yield return ContentLoader.LoadSprite(ContentKeys.Art(L1HelloFriends.PropTree), ContentKeys.Art(L1HelloFriends.PropTree), s => _treeSprite = s);
            yield return ContentLoader.LoadSprite(ContentKeys.Art(L1HelloFriends.PropRock), ContentKeys.Art(L1HelloFriends.PropRock), s => _rockSprite = s);
            yield return ContentLoader.LoadSprite(ContentKeys.Art(L1HelloFriends.PropBall), ContentKeys.Art(L1HelloFriends.PropBall), s => _ballSprite = s);
            yield return ContentLoader.LoadSprite(ContentKeys.Art(L1HelloFriends.DropFox), ContentKeys.Art(L1HelloFriends.DropFox), s => _dropSprite = s);
            yield return ContentLoader.LoadSprite(ContentKeys.Art(L1HelloFriends.AnimBunnyWave), ContentKeys.Art(L1HelloFriends.AnimBunnyWave), s => _animBunnyWave = s);
            yield return ContentLoader.LoadSprite(ContentKeys.Art(L1HelloFriends.AnimWaveBunny), ContentKeys.Art(L1HelloFriends.AnimWaveBunny), s => _animWaveBunny = s);
            yield return ContentLoader.LoadSprite(ContentKeys.Art(L1HelloFriends.AnimFoxWave), ContentKeys.Art(L1HelloFriends.AnimFoxWave), s => _animFoxWave = s);
            yield return ContentLoader.LoadSprite(ContentKeys.Art(L1HelloFriends.AnimFoxCatch), ContentKeys.Art(L1HelloFriends.AnimFoxCatch), s => _animFoxCatch = s);
            yield return ContentLoader.LoadSprite(ContentKeys.Art(ContentKeys.Friend), ContentKeys.Art(ContentKeys.Friend), s => _friendSprite = s);
            if (_bunnySprite == null)
                yield return ContentLoader.LoadSprite(ContentKeys.Art(ContentKeys.Hi), ContentKeys.Art(ContentKeys.Hi), s => _bunnySprite = s);
            if (_foxSprite == null)
                yield return ContentLoader.LoadSprite(ContentKeys.Art(ContentKeys.Come), ContentKeys.Art(ContentKeys.Come), s => _foxSprite = s);

            if (_successClip == null)
                _successClip = WavUtility.MakeTone(L1HelloFriends.SfxStar, new[] { 523.25f, 659.25f, 783.99f }, 0.45f);
            if (_retryClip == null)
                _retryClip = WavUtility.MakeTone(L1HelloFriends.SfxWrongSoft, new[] { 329.63f, 261.63f }, 0.28f);
            if (_byeClip == null)
                _byeClip = WavUtility.MakeTone(L1HelloFriends.VoByeExpose, new[] { 392f, 329.63f, 261.63f }, 0.9f);
            if (_speakClip == null)
                _speakClip = _hiClip;
        }

        void ApplyArt()
        {
            if (_parkBg != null)
            {
                if (_bgSprite != null)
                {
                    _parkBg.sprite = _bgSprite;
                    _parkBg.color = Color.white;
                    _parkBg.preserveAspect = false;
                }
                else
                    _parkBg.color = DesignTokens.Bg;
            }

            _bunny.SetPortrait(_bunnySprite, BunnyTint);
            _fox.SetPortrait(_foxSprite != null ? _foxSprite : _dropSprite, FoxTint);
            _friend.SetPortrait(_friendSprite, new Color(0.64f, 0.80f, 0.95f));
            _tree.SetPortrait(_treeSprite, new Color(0.46f, 0.72f, 0.42f));
            _rock.SetPortrait(_rockSprite, new Color(0.62f, 0.60f, 0.56f));
            _ball.SetPortrait(_ballSprite);
        }

        void BuildUi()
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
                canvas = FindAnyObjectByType<Canvas>();
            _canvasRt = canvas.GetComponent<RectTransform>();
            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
                KidUi.ApplyPortraitCanvas(scaler);

            var root = KidUi.Panel(_canvasRt, "HelloFriendsRoot", DesignTokens.Bg);
            KidUi.Stretch(root);

            var title = KidUi.Label(root, "Title", "Hello, Friends!", DesignTokens.FontTitleSp, DesignTokens.Text);
            title.rectTransform.anchorMin = new Vector2(0.06f, 0.91f);
            title.rectTransform.anchorMax = new Vector2(0.78f, 0.99f);
            title.rectTransform.offsetMin = title.rectTransform.offsetMax = Vector2.zero;

            var lockBtn = ParentEntryButton.Create(root);
            lockBtn.LongPressed += () => _parent?.OpenFromLongPress();
            lockBtn.Tapped += () => FindAnyObjectByType<ParentPanelView>()?.ShowPinPad();

            var listenHit = DesignTokens.ListenDemoDp + DesignTokens.HitSlopDp * 2f;
            _listenRt = KidUi.Panel(root, L1HelloFriends.UiBtnListen, new Color(1f, 1f, 1f, 0.02f));
            _listenRt.anchorMin = new Vector2(0.08f, 0.79f);
            _listenRt.anchorMax = new Vector2(0.92f, 0.79f);
            _listenRt.pivot = new Vector2(0.5f, 0.5f);
            _listenRt.sizeDelta = new Vector2(0f, listenHit);
            _listenRt.offsetMin = new Vector2(DesignTokens.ScreenPadDp, _listenRt.offsetMin.y);
            _listenRt.offsetMax = new Vector2(-DesignTokens.ScreenPadDp, _listenRt.offsetMax.y);
            _listenVisual = KidUi.Panel(_listenRt, "Visual", DesignTokens.Primary).GetComponent<Image>();
            KidUi.Stretch(_listenVisual.rectTransform, 0, 0, DesignTokens.HitSlopDp, DesignTokens.HitSlopDp);
            _listenVisual.raycastTarget = false;
            _listenCaption = KidUi.Label(_listenRt, "Caption", "听一听  Listen", DesignTokens.FontBodySp, Color.white);
            var listenBtn = _listenRt.gameObject.AddComponent<Button>();
            listenBtn.targetGraphic = _listenRt.GetComponent<Image>();
            listenBtn.onClick.AddListener(OnListenDemo);

            _status = KidUi.Label(root, "Status", "Hello, Friends!", DesignTokens.FontBodySp, DesignTokens.Text, FontStyle.Normal);
            _status.rectTransform.anchorMin = new Vector2(0.06f, 0.70f);
            _status.rectTransform.anchorMax = new Vector2(0.94f, 0.78f);
            _status.rectTransform.offsetMin = _status.rectTransform.offsetMax = Vector2.zero;

            var stage = KidUi.Panel(root, "ParkGate", Color.clear);
            stage.anchorMin = new Vector2(0.04f, 0.12f);
            stage.anchorMax = new Vector2(0.96f, 0.70f);
            stage.offsetMin = stage.offsetMax = Vector2.zero;
            _parkBg = stage.GetComponent<Image>();
            _parkBg.color = DesignTokens.Bg;
            _parkBg.raycastTarget = false;
            stage.gameObject.name = L1HelloFriends.BgParkGate;

            _bunny = CharacterHotspot.CreatePark(stage, L1HelloFriends.HitBunny, L1HelloFriends.Bunny, L1HelloFriends.CharBunny, "🐰 Bunny  兔子", "Bunny", new Color(0.97f, 0.73f, 0.82f), L1HelloFriends.ParkBunny, L1HelloFriends.HitSizeBunny, null);
            _tree = CharacterHotspot.CreatePark(stage, L1HelloFriends.HitTree, L1HelloFriends.Tree, L1HelloFriends.PropTree, "🌳 Tree  树", "Tree", new Color(0.46f, 0.72f, 0.42f), L1HelloFriends.ParkTree, L1HelloFriends.HitSizeTree, null);
            _rock = CharacterHotspot.CreatePark(stage, L1HelloFriends.HitRock, L1HelloFriends.Rock, L1HelloFriends.PropRock, "🪨 Rock  石头", "Rock", new Color(0.62f, 0.60f, 0.56f), L1HelloFriends.ParkRock, L1HelloFriends.HitSizeRock, null);
            _fox = CharacterHotspot.CreatePark(stage, L1HelloFriends.DropFox, L1HelloFriends.Fox, L1HelloFriends.CharFox, "🦊 Fox  狐狸", "Fox", new Color(0.98f, 0.57f, 0.24f), L1HelloFriends.ParkFox, L1HelloFriends.HitSizeFox, null);
            _friend = CharacterHotspot.CreatePark(stage, "Friend", L1HelloFriends.Friend, "char_friend", "Friend  朋友", "Friend", new Color(0.64f, 0.80f, 0.95f), L1HelloFriends.ParkFriend, L1HelloFriends.HitSizeFriend, null);
            _fox.SetVisible(false);
            _friend.SetVisible(false);
            _choiceSpots = new[] { _bunny, _tree, _rock };
            _hotspots = new[] { _bunny, _tree, _rock, _fox, _friend };
            foreach (var spot in _hotspots)
            {
                foreach (var banned in L1HelloFriends.ForbiddenInThisLevel)
                {
                    if (spot != null && spot.OptionId == banned)
                        Debug.LogError("[KidsEnglish] L1 v1 HARD LOCK forbids optionId " + banned);
                }
            }

            foreach (var spot in _choiceSpots)
                spot.Tapped += OnHotspotTapped;
            _fox.Tapped += OnFoxTapToSend;

            _ball = InviteBall.Create(stage);
            _ball.Bind(_fox);
            _ball.DroppedOn += OnBallDropped;
            _ball.Tapped += OnBallTapped;

            _star = KidUi.Label(root, "Star", "", 36, DesignTokens.Success);
            _star.rectTransform.anchorMin = new Vector2(0.06f, 0.02f);
            _star.rectTransform.anchorMax = new Vector2(0.20f, 0.12f);
            _star.rectTransform.offsetMin = _star.rectTransform.offsetMax = Vector2.zero;
            _star.gameObject.SetActive(false);

            BuildProgressDots(root);
        }

        void BuildProgressDots(RectTransform root)
        {
            var holder = KidUi.Panel(root, L1HelloFriends.UiProgressDots, Color.clear);
            holder.anchorMin = new Vector2(0.32f, 0.035f);
            holder.anchorMax = new Vector2(0.68f, 0.09f);
            holder.offsetMin = holder.offsetMax = Vector2.zero;
            holder.GetComponent<Image>().raycastTarget = false;
            _progressDots = new Image[4];
            for (var i = 0; i < 4; i++)
            {
                var dot = KidUi.Panel(holder, "Dot" + i, new Color(1f, 1f, 1f, 0.55f));
                dot.anchorMin = dot.anchorMax = new Vector2(0.14f + i * 0.24f, 0.5f);
                dot.sizeDelta = new Vector2(14f, 14f);
                dot.GetComponent<Image>().raycastTarget = false;
                _progressDots[i] = dot.GetComponent<Image>();
            }
        }

        void SetProgressDots(int lit)
        {
            if (_progressDots == null)
                return;
            for (var i = 0; i < _progressDots.Length; i++)
            {
                if (_progressDots[i] == null)
                    continue;
                _progressDots[i].color = i <= lit ? DesignTokens.Primary : new Color(1f, 1f, 1f, 0.45f);
            }
        }

        static int ProgressDotFor(L1Beat beat)
        {
            switch (beat)
            {
                case L1Beat.DemoCome:
                case L1Beat.TaskInvite:
                    return 1;
                case L1Beat.OptionalRec:
                    return 2;
                case L1Beat.ByeBunny:
                case L1Beat.ByeFox:
                case L1Beat.Outro:
                    return 3;
                default:
                    return 0;
            }
        }

        void EnterBeat(L1Beat beat)
        {
            _beat = beat;
            _busy = false;
            ClearWarn();
            SetProgressDots(ProgressDotFor(beat));

            switch (beat)
            {
                case L1Beat.Intro:
                    ShowIntroStage();
                    if (_bunny != null)
                        StartCoroutine(PlayLockedAnim(_bunny, _bunnySprite, BunnyTint, _animBunnyWave, UiMotion.Wave(_bunny.Rect)));
                    RaiseFeedback(FeedbackKind.Intro, "l1.intro", "Who says Hi?");
                    SetListenChrome(ListenUiState.WaitingToListen, "听一听  Listen", "Who says Hi?");
                    break;
                case L1Beat.DemoHi:
                    ShowChoiceStage();
                    RaiseFeedback(FeedbackKind.Demo, "l1.demo_hi", "Listen: Hi");
                    PlayThen(L1Beat.ChoiceHi, _hiClip, L1HelloFriends.ClipHi, L1HelloFriends.Bunny, "Hi", assess: false);
                    break;
                case L1Beat.ChoiceHi:
                    ShowChoiceStage();
                    SetListenChrome(ListenUiState.WaitingChoice, "再听  Hi", StatusForChoiceHi());
                    break;
                case L1Beat.DemoCome:
                    ShowInviteStage(showBall: false);
                    RaiseFeedback(FeedbackKind.Demo, "l1.demo_come", "Listen: Come play!");
                    PlayThen(L1Beat.TaskInvite, _comeClip, L1HelloFriends.ClipComePlay, L1HelloFriends.Fox, "Come play!", assess: false);
                    break;
                case L1Beat.TaskInvite:
                    ShowInviteStage(showBall: true);
                    _inviteFails = 0;
                    _tapHintMode = false;
                    _ball.SetTapHintMode(false);
                    SetListenChrome(ListenUiState.WaitingChoice, "再听  Come play", StatusForInvite());
                    break;
                case L1Beat.OptionalRec:
                    MarkLevelClearIfNeeded();
                    SetListenChrome(ListenUiState.WaitingChoice, "听一听  Hi", "Hi!");
                    ApplyPhaseLock();
                    ApplyScaffold();
                    if (!OpenOptionalSpeak())
                        return;
                    break;
                case L1Beat.ByeBunny:
                    MarkLevelClearIfNeeded();
                    if (_demoRoutine != null)
                        StopCoroutine(_demoRoutine);
                    _demoRoutine = StartCoroutine(ByeRequiredRoutine(
                        L1HelloFriends.ClipByeBunny,
                        L1HelloFriends.Bunny,
                        L1Beat.ByeFox));
                    return;
                case L1Beat.ByeFox:
                    MarkLevelClearIfNeeded();
                    if (_demoRoutine != null)
                        StopCoroutine(_demoRoutine);
                    _demoRoutine = StartCoroutine(ByeRequiredRoutine(
                        L1HelloFriends.ClipByeFox,
                        L1HelloFriends.Fox,
                        L1Beat.Outro));
                    return;
                case L1Beat.Outro:
                    MarkLevelClearIfNeeded();
                    _ball.Show(false);
                    _star.text = "★";
                    _star.gameObject.SetActive(true);
                    SetListenChrome(ListenUiState.Correct, "★", "done");
                    RaiseFeedback(FeedbackKind.Outro, "l1.outro", "done");
                    StartCoroutine(UiMotion.Celebrate(_star.rectTransform));
                    break;
            }

            ApplyPhaseLock();
            ApplyScaffold();
        }

        void PlayThen(L1Beat next, AudioClip clip, string clipId, string targetId, string caption, bool assess = false)
        {
            if (_demoRoutine != null)
                StopCoroutine(_demoRoutine);
            _demoRoutine = StartCoroutine(PlayDemoRoutine(next, clip, clipId, targetId, caption, assess));
        }

        IEnumerator PlayDemoRoutine(L1Beat next, AudioClip clip, string clipId, string targetId, string caption, bool assess)
        {
            _busy = true;
            ClearWarn();
            SetListenChrome(ListenUiState.Playing, "♪  " + caption, "Playing…  正在听");
            ApplyScaffold();
            yield return UiMotion.Tap(_listenRt);
            _audio.PlayDemo(clip, LevelId, clipId, targetId, canReplay: true, assess: assess);
            var wait = clip != null ? Mathf.Max(0.4f, clip.length) : 1.2f;
            yield return new WaitForSeconds(wait);
            _busy = false;
            _demoRoutine = null;
            if (_beat == L1Beat.DemoHi || _beat == L1Beat.DemoCome)
                EnterBeat(next);
            else if (_beat == L1Beat.ChoiceHi || _beat == L1Beat.TaskInvite)
            {
                var status = _beat == L1Beat.TaskInvite ? StatusForInvite() : StatusForChoiceHi();
                SetListenChrome(ListenUiState.WaitingChoice, "再听  " + caption, status);
            }
        }

        void OnListenDemo()
        {
            if (_listenState == ListenUiState.Playing)
                return;

            switch (_beat)
            {
                case L1Beat.Intro:
                    EnterBeat(L1Beat.DemoHi);
                    break;
                case L1Beat.ChoiceHi:
                    PlayThen(L1Beat.ChoiceHi, _hiClip, L1HelloFriends.ClipHi, L1HelloFriends.Bunny, "Hi", assess: false);
                    break;
                case L1Beat.TaskInvite:
                    PlayThen(L1Beat.TaskInvite, _comeClip, L1HelloFriends.ClipComePlay, L1HelloFriends.Fox, "Come play!", assess: false);
                    break;
                case L1Beat.DemoHi:
                case L1Beat.DemoCome:
                case L1Beat.OptionalRec:
                case L1Beat.ByeBunny:
                case L1Beat.ByeFox:
                case L1Beat.Outro:
                    break;
            }
        }

        void OnHotspotTapped(CharacterHotspot spot)
        {
            if (L1HelloFriends.PhaseOf(_beat) != L1Phase.ThreeChoice || _busy)
                return;
            if (_listenState != ListenUiState.WaitingChoice && _listenState != ListenUiState.Wrong)
                return;

            StartCoroutine(HandleTap(spot));
        }

        IEnumerator HandleTap(CharacterHotspot spot)
        {
            _busy = true;
            yield return UiMotion.Tap(spot.Rect);
            _audio.StopDemo();
            var correct = spot.OptionId == L1HelloFriends.Bunny;
            GameEventBus.RaiseChoice(new ChoicePayload
            {
                LevelId = LevelId,
                ChoiceId = L1HelloFriends.ChoiceHi,
                OptionId = spot.OptionId,
                Correct = correct
            });

            if (correct)
            {
                SetListenChrome(ListenUiState.Correct, "★", "You did it!  太棒了！");
                _audio.PlaySfx(_successClip);
                _star.text = "★";
                _star.gameObject.SetActive(true);
                RaiseScaffoldOnSuccess();
                RaiseFeedback(FeedbackKind.Success, "l1.hi.success", "Hi — Bunny!");
                _progress?.LightIdentify(ParentMetrics.IdentifyHi);
                ApplyPhaseLock();
                yield return UiMotion.Celebrate(_star.rectTransform);
                _busy = false;
                EnterBeat(L1Beat.DemoCome);
            }
            else
            {
                SetListenChrome(ListenUiState.Wrong, "再听  Hi", "Almost! Listen again  再听一次吧");
                _audio.PlaySfx(_retryClip);
                spot.ShowWarn(true);
                DropScaffoldToFull();
                RaiseFeedback(FeedbackKind.Retry, "l1.hi.retry", "Almost! Try Bunny");
                yield return UiMotion.Shake(spot.Rect);
                _busy = false;
                ApplyScaffold();
            }
        }

        void OnFoxTapToSend(CharacterHotspot _)
        {
            if (L1HelloFriends.PhaseOf(_beat) != L1Phase.BallFox || _busy)
                return;
            StartCoroutine(HandleDrop(L1HelloFriends.Fox));
        }

        void OnBallTapped()
        {
            if (L1HelloFriends.PhaseOf(_beat) != L1Phase.BallFox || _busy)
                return;
            StartCoroutine(HandleDrop(L1HelloFriends.Fox));
        }

        void OnBallDropped(string optionId)
        {
            if (L1HelloFriends.PhaseOf(_beat) != L1Phase.BallFox || _busy)
            {
                _ball.SnapHome();
                return;
            }

            if (string.IsNullOrEmpty(optionId) || optionId != L1HelloFriends.Fox)
            {
                StartCoroutine(MissInvite());
                return;
            }

            StartCoroutine(HandleDrop(L1HelloFriends.Fox));
        }

        IEnumerator MissInvite()
        {
            _busy = true;
            _ball.SetDraggingEnabled(false);
            _fox.SetTappable(false);
            _inviteFails++;
            if (_inviteFails >= 2)
            {
                _tapHintMode = true;
                _ball.SetTapHintMode(true);
            }

            SetListenChrome(ListenUiState.Wrong, "再听  Come play", StatusForInvite());
            _audio.PlaySfx(_retryClip);
            DropScaffoldToFull();
            RaiseFeedback(FeedbackKind.Retry, "l1.come.retry", "Almost! Invite Fox");
            yield return _ball.ShakeHome();
            _busy = false;
            ApplyPhaseLock();
            ApplyScaffold();
        }

        IEnumerator HandleDrop(string optionId)
        {
            _busy = true;
            _ball.SetDraggingEnabled(false);
            _fox.SetTappable(false);
            _audio.StopDemo();
            var correct = optionId == L1HelloFriends.Fox;
            GameEventBus.RaiseChoice(new ChoicePayload
            {
                LevelId = LevelId,
                ChoiceId = L1HelloFriends.ChoiceComePlay,
                OptionId = optionId,
                Correct = correct
            });

            var spot = FindSpot(optionId);
            if (correct)
            {
                yield return _ball.FlyTo(_fox.Rect);
                SetListenChrome(ListenUiState.Correct, "★", "Come play!  一起来玩！");
                _audio.PlaySfx(_successClip);
                _star.text = "★";
                _star.gameObject.SetActive(true);
                _inviteFails = 0;
                RaiseScaffoldOnSuccess();
                RaiseFeedback(FeedbackKind.Success, "l1.come.success", "Come play — Fox!");
                _progress?.LightIdentify(ParentMetrics.IdentifyPlayCome);
                if (_fox != null)
                    StartCoroutine(PlayLockedAnim(_fox, _foxSprite != null ? _foxSprite : _dropSprite, FoxTint, _animFoxCatch, UiMotion.Catch(_fox.Rect)));
                yield return UiMotion.Celebrate(_star.rectTransform);
                _inviteCleared = true;
                _ball.Show(false);
                _busy = false;
                EnterBeat(L1Beat.OptionalRec);
            }
            else
            {
                _inviteFails++;
                if (_inviteFails >= 2)
                {
                    _tapHintMode = true;
                    _ball.SetTapHintMode(true);
                }

                SetListenChrome(ListenUiState.Wrong, "再听  Come play", StatusForInvite());
                _audio.PlaySfx(_retryClip);
                if (spot != null)
                    spot.ShowWarn(true);
                DropScaffoldToFull();
                RaiseFeedback(FeedbackKind.Retry, "l1.come.retry", "Almost! Invite Fox");
                yield return _ball.ShakeHome();
                _busy = false;
                ApplyPhaseLock();
                ApplyScaffold();
            }
        }

        bool OpenOptionalSpeak()
        {
            var speakAlong = _progress == null || _progress.Data.speakAlongEnabled;
            if (!speakAlong || _speak == null)
            {
                EnterBeat(L1Beat.ByeBunny);
                return false;
            }

            _speak.BlockProgress = false;
            _speak.ScaffoldLevel = _scaffold;
            _speak.Bind(
                LevelId,
                "Hi",
                L1HelloFriends.ClipHi,
                _speakClip != null ? _speakClip : (_hiClip != null ? _hiClip : _comeClip),
                _audio,
                _recorder,
                _scorer,
                L1HelloFriends.UtteranceHi);
            _speak.ShowIdle();
            return true;
        }

        void OnSpeakFinished()
        {
            if (_beat != L1Beat.OptionalRec)
                return;
            if (_speak != null && _speak.gameObject.activeSelf)
                _speak.Hide();
            EnterBeat(L1Beat.ByeBunny);
        }

        void OnRecordStopSave(RecordStopPayload payload)
        {
            if (payload.LevelId != LevelId || _progress == null)
                return;
            _progress.SetLastRecord(payload.Score.ToString("0.00"), payload.AudioRef);
            if (payload.UtteranceId == L1HelloFriends.UtteranceHi)
                _progress.LightSpeak(ParentMetrics.SpeakHi);
        }

        void MarkLevelClearIfNeeded()
        {
            if (!_inviteCleared)
                return;
            _progress?.MarkHelloFriendsComplete(null, null);
        }

        void RaiseScaffoldOnSuccess()
        {
            if (_scaffold < L1HelloFriends.ScaffoldWithdrawn)
                _scaffold++;
        }

        void DropScaffoldToFull()
        {
            _scaffold = L1HelloFriends.ScaffoldFull;
        }

        void ApplyScaffold()
        {
            var hintId = (string)null;
            if (_beat == L1Beat.ChoiceHi)
                hintId = L1HelloFriends.Bunny;
            else if (_beat == L1Beat.TaskInvite)
                hintId = L1HelloFriends.Fox;
            else if (_beat == L1Beat.DemoHi)
                hintId = L1HelloFriends.Bunny;
            else if (_beat == L1Beat.DemoCome)
                hintId = L1HelloFriends.Fox;
            else if (_beat == L1Beat.Intro)
                hintId = L1HelloFriends.Friend;
            else if (_beat == L1Beat.ByeBunny)
                hintId = L1HelloFriends.Bunny;
            else if (_beat == L1Beat.ByeFox)
                hintId = L1HelloFriends.Fox;

            foreach (var spot in _hotspots)
            {
                if (spot == null || !spot.gameObject.activeSelf)
                    continue;
                spot.ApplyScaffold(_scaffold, hintId == spot.OptionId && _listenState != ListenUiState.Playing);
            }

            _ball.ApplyScaffold(_scaffold, (_beat == L1Beat.TaskInvite && _listenState != ListenUiState.Playing) || _tapHintMode);
        }

        IEnumerator PlayLockedAnim(CharacterHotspot spot, Sprite idle, Color fallback, Sprite frame, IEnumerator motion)
        {
            if (spot == null)
                yield break;
            if (frame != null)
                spot.SetPortrait(frame, fallback);
            yield return motion;
            spot.SetPortrait(idle, fallback);
        }

        IEnumerator ByeRequiredRoutine(string clipId, string targetId, L1Beat next)
        {
            ShowByeStage();
            ApplyPhaseLock();
            ApplyScaffold();
            RaiseFeedback(FeedbackKind.Demo, "l1." + clipId, "Bye!");
            SetListenChrome(ListenUiState.Playing, "Bye!", "Bye!");
            var waver = targetId == L1HelloFriends.Fox ? _fox : _bunny;
            if (waver != null)
            {
                var idle = targetId == L1HelloFriends.Fox
                    ? (_foxSprite != null ? _foxSprite : _dropSprite)
                    : _bunnySprite;
                var tint = targetId == L1HelloFriends.Fox ? FoxTint : BunnyTint;
                var frame = targetId == L1HelloFriends.Fox ? _animFoxWave : _animWaveBunny;
                if (frame == null && targetId == L1HelloFriends.Bunny)
                    frame = _animBunnyWave;
                StartCoroutine(PlayLockedAnim(waver, idle, tint, frame, UiMotion.Wave(waver.Rect)));
            }
            _audio.PlayDemo(_byeClip, LevelId, clipId, targetId, canReplay: false, assess: false);
            _progress?.LightIdentify(ParentMetrics.IdentifyByeWave);
            var wait = _byeClip != null ? Mathf.Max(0.6f, _byeClip.length) : 1.0f;
            yield return new WaitForSeconds(wait);
            _demoRoutine = null;
            if (_beat == L1Beat.ByeBunny || _beat == L1Beat.ByeFox)
                EnterBeat(next);
        }

        void ShowIntroStage()
        {
            _bunny.SetAnchor(L1HelloFriends.ParkBunny);
            _bunny.SetVisible(true);
            _tree.SetAnchor(L1HelloFriends.ParkTree);
            _tree.SetVisible(true);
            _rock.SetAnchor(L1HelloFriends.ParkRock);
            _rock.SetVisible(true);
            _friend.SetAnchor(L1HelloFriends.ParkFriend);
            _friend.SetVisible(true);
            _fox.SetVisible(false);
            _ball.Show(false);
        }

        void ShowByeStage()
        {
            _bunny.SetAnchor(L1HelloFriends.ParkBunny);
            _bunny.SetVisible(true);
            _fox.SetAnchor(L1HelloFriends.ParkFox);
            _fox.SetVisible(true);
            if (_friend != null)
                _friend.SetVisible(false);
            _tree.SetVisible(true);
            _rock.SetVisible(true);
            _ball.Show(false);
        }

        void ShowChoiceStage()
        {
            _bunny.SetAnchor(L1HelloFriends.ParkBunny);
            _bunny.SetVisible(true);
            _tree.SetAnchor(L1HelloFriends.ParkTree);
            _tree.SetVisible(true);
            _rock.SetAnchor(L1HelloFriends.ParkRock);
            _rock.SetVisible(true);
            _fox.SetVisible(false);
            _ball.Show(false);
            if (_friend != null)
                _friend.SetVisible(false);
        }

        void ShowInviteStage(bool showBall)
        {
            _bunny.SetAnchor(L1HelloFriends.ParkBunny);
            _bunny.SetVisible(true);
            _tree.SetAnchor(L1HelloFriends.ParkTree);
            _tree.SetVisible(true);
            _rock.SetAnchor(L1HelloFriends.ParkRock);
            _rock.SetVisible(true);
            _fox.SetAnchor(L1HelloFriends.ParkFox);
            _fox.SetVisible(true);
            if (_friend != null)
                _friend.SetVisible(false);
            _ball.Show(showBall);
        }

        void ApplyPhaseLock()
        {
            var phase = L1HelloFriends.PhaseOf(_beat);
            var choiceOn = phase == L1Phase.ThreeChoice;
            var inviteOn = phase == L1Phase.BallFox;

            foreach (var spot in _choiceSpots)
            {
                if (spot != null)
                    spot.SetTappable(choiceOn);
            }

            if (_fox != null)
                _fox.SetTappable(inviteOn);
            if (_friend != null)
                _friend.SetTappable(false);
            if (_ball != null)
                _ball.SetDraggingEnabled(inviteOn);
            if (_listenRt != null)
            {
                var listenBtn = _listenRt.GetComponent<Button>();
                if (listenBtn != null)
                    listenBtn.interactable = phase != L1Phase.WrapUp;
            }
        }

        void ClearWarn()
        {
            if (_hotspots == null)
                return;
            foreach (var spot in _hotspots)
            {
                if (spot != null)
                    spot.ShowWarn(false);
            }
        }

        CharacterHotspot FindSpot(string optionId)
        {
            foreach (var spot in _hotspots)
            {
                if (spot.OptionId == optionId)
                    return spot;
            }

            return null;
        }

        string StatusForChoiceHi()
        {
            if (_scaffold <= L1HelloFriends.ScaffoldFull)
                return "Who says Hi?  点点兔子";
            if (_scaffold == L1HelloFriends.ScaffoldReduced)
                return "Who says Hi?";
            return "Who says Hi?";
        }

        string StatusForInvite()
        {
            if (_tapHintMode)
                return "Come play!  点点球";
            return "Come play!";
        }

        void SetListenChrome(ListenUiState state, string caption, string status)
        {
            _listenState = state;
            if (_pulseListen != null)
            {
                StopCoroutine(_pulseListen);
                _pulseListen = null;
                if (_listenRt != null)
                    _listenRt.localScale = Vector3.one;
            }

            _listenCaption.text = caption;
            _status.text = status;

            switch (state)
            {
                case ListenUiState.WaitingToListen:
                case ListenUiState.WaitingChoice:
                    _status.color = DesignTokens.Text;
                    _listenVisual.color = DesignTokens.Primary;
                    break;
                case ListenUiState.Playing:
                    _status.color = DesignTokens.Primary;
                    _listenVisual.color = DesignTokens.Primary;
                    break;
                case ListenUiState.Correct:
                    _status.color = DesignTokens.Success;
                    _listenVisual.color = DesignTokens.Success;
                    break;
                case ListenUiState.Wrong:
                    _status.color = DesignTokens.SoftWarn;
                    _listenVisual.color = DesignTokens.SoftWarn;
                    _pulseListen = StartCoroutine(UiMotion.Pulse(_listenRt, () => _listenState == ListenUiState.Wrong));
                    break;
            }

            if (state == ListenUiState.WaitingToListen)
                _pulseListen = StartCoroutine(UiMotion.Pulse(_listenRt, () => _listenState == ListenUiState.WaitingToListen));
        }

        void RaiseFeedback(string kind, string textKey, string message)
        {
            GameEventBus.RaiseFeedback(new FeedbackPayload
            {
                LevelId = LevelId,
                Kind = kind,
                TextKey = textKey,
                ScaffoldLevel = _scaffold,
                Message = message
            });
        }

        void EnsureSceneShell()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera");
                cam = go.AddComponent<Camera>();
                go.tag = "MainCamera";
                if (FindAnyObjectByType<AudioListener>() == null)
                    go.AddComponent<AudioListener>();
            }

            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.backgroundColor = DesignTokens.Bg;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.GetUniversalAdditionalCameraData();

            if (FindAnyObjectByType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

            if (FindAnyObjectByType<Canvas>() == null)
            {
                var canvasGo = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                var canvas = canvasGo.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                KidUi.ApplyPortraitCanvas(canvasGo.GetComponent<CanvasScaler>());
            }
        }
    }
}
