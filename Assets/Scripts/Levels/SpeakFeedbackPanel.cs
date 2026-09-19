using System;
using System.Collections;
using KidsEnglish.Audio;
using KidsEnglish.Core;
using UnityEngine;
using UnityEngine.UI;

namespace KidsEnglish.Levels
{
    /// <summary>
    /// SpeakFeedback (跟读) panel. Layout: sentence + 再听 + mic ≥96dp + star bar + 再试/跳过.
    /// States: idle → listening (5–8s) → processing (≤1.5s) → success / retry.
    /// Optional speak-along uses blockProgress=false so Skip/success never gates the level.
    /// GameObject name is SpeakFeedback (Art P0 UI reuse).
    /// </summary>
    public sealed class SpeakFeedbackPanel : MonoBehaviour
    {
        public event Action Closed;
        public event Action Skipped;
        public event Action Succeeded;

        public bool BlockProgress { get; set; }
        public int ScaffoldLevel { get; set; } = 1;

        string _levelId;
        string _sentence;
        string _clipId;
        string _utteranceId;
        AudioClip _demoClip;
        AudioPlayer _audio;
        LocalMicRecorder _recorder;
        ISpeechScorer _scorer;

        SpeakUiState _state = SpeakUiState.Idle;
        Text _sentenceLabel;
        Text _status;
        Text _micCaption;
        Image _micVisual;
        Text[] _starLabels;
        Button _listenAgain;
        Button _mic;
        Button _retry;
        Button _skip;
        Coroutine _flow;
        bool _stopListen;

        public SpeakUiState State => _state;

        public static SpeakFeedbackPanel Create(Transform canvas)
        {
            var go = new GameObject("SpeakFeedback", typeof(RectTransform), typeof(SpeakFeedbackPanel));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(canvas, false);
            KidUi.Stretch(rt);
            var panel = go.GetComponent<SpeakFeedbackPanel>();
            panel.Build();
            panel.Hide();
            return panel;
        }

        void Awake()
        {
            if (transform.childCount == 0)
                Build();
        }

        public void Bind(
            string levelId,
            string sentence,
            string clipId,
            AudioClip demoClip,
            AudioPlayer audio,
            LocalMicRecorder recorder,
            ISpeechScorer scorer,
            string utteranceId = "hi")
        {
            _levelId = levelId;
            _sentence = sentence;
            _clipId = clipId;
            _utteranceId = string.IsNullOrEmpty(utteranceId) ? "hi" : utteranceId;
            _demoClip = demoClip;
            _audio = audio;
            _recorder = recorder;
            _scorer = scorer;
            if (_sentenceLabel != null)
                _sentenceLabel.text = sentence;
        }

        public void ShowIdle()
        {
            gameObject.SetActive(true);
            SetState(SpeakUiState.Idle);
        }

        public void Hide()
        {
            if (_recorder != null && _recorder.IsRecording)
                _recorder.StopCapture();
            gameObject.SetActive(false);
            Closed?.Invoke();
        }

        void Build()
        {
            var dim = KidUi.Panel(transform, "Dim", new Color(0.12f, 0.25f, 0.4f, 0.35f));
            KidUi.Stretch(dim);

            var sheet = KidUi.Panel(transform, "Sheet", DesignTokens.Card);
            sheet.anchorMin = new Vector2(0.04f, 0.08f);
            sheet.anchorMax = new Vector2(0.96f, 0.92f);
            sheet.offsetMin = sheet.offsetMax = Vector2.zero;

            _sentenceLabel = KidUi.Label(sheet, "Sentence", "Hi", DesignTokens.FontTitleSp, DesignTokens.Text);
            Place(sheet, _sentenceLabel.rectTransform, 0.78f, 0.96f);

            _listenAgain = KidUi.BigButton(sheet, "ListenAgain", "再听", DesignTokens.Primary, OnListenAgain, new Vector2(160f, 56f));
            PlaceCenter(_listenAgain.GetComponent<RectTransform>(), new Vector2(0.5f, 0.72f));
            ColorButtonCaption(_listenAgain, Color.white);

            var micHit = DesignTokens.MicDp + DesignTokens.HitSlopDp * 2f;
            var micRt = KidUi.Panel(sheet, "Mic", new Color(1f, 1f, 1f, 0.02f));
            micRt.anchorMin = micRt.anchorMax = new Vector2(0.5f, 0.50f);
            micRt.sizeDelta = new Vector2(micHit, micHit);
            _micVisual = KidUi.Panel(micRt, "Visual", DesignTokens.Primary).GetComponent<Image>();
            _micVisual.rectTransform.anchorMin = _micVisual.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _micVisual.rectTransform.sizeDelta = new Vector2(DesignTokens.MicDp, DesignTokens.MicDp);
            _micVisual.raycastTarget = false;
            _micCaption = KidUi.Label(micRt, "Caption", "🎤", 36, Color.white);
            _mic = micRt.gameObject.AddComponent<Button>();
            _mic.targetGraphic = micRt.GetComponent<Image>();
            _mic.onClick.AddListener(OnMic);

            var starRow = KidUi.Panel(sheet, "Stars", Color.clear);
            Place(sheet, starRow, 0.30f, 0.40f);
            starRow.GetComponent<Image>().raycastTarget = false;
            _starLabels = new Text[3];
            for (var i = 0; i < 3; i++)
            {
                var star = KidUi.Label(starRow, "Star" + i, "☆", 28, DesignTokens.TextMuted);
                star.rectTransform.anchorMin = new Vector2(i / 3f, 0f);
                star.rectTransform.anchorMax = new Vector2((i + 1) / 3f, 1f);
                star.rectTransform.offsetMin = star.rectTransform.offsetMax = Vector2.zero;
                _starLabels[i] = star;
            }

            _status = KidUi.Label(sheet, "Status", "Optional — say Hi", DesignTokens.FontBodySp, DesignTokens.TextMuted, FontStyle.Normal);
            Place(sheet, _status.rectTransform, 0.20f, 0.30f);

            _retry = KidUi.BigButton(sheet, "Retry", "再试", DesignTokens.SoftWarn, OnRetry, new Vector2(120f, 56f));
            PlaceCenter(_retry.GetComponent<RectTransform>(), new Vector2(0.32f, 0.10f));

            _skip = KidUi.BigButton(sheet, "Skip", "跳过", new Color(0.90f, 0.94f, 1f, 1f), OnSkip, new Vector2(120f, 56f));
            PlaceCenter(_skip.GetComponent<RectTransform>(), new Vector2(0.68f, 0.10f));
        }

        static void Place(RectTransform parent, RectTransform child, float yMin, float yMax)
        {
            child.SetParent(parent, false);
            child.anchorMin = new Vector2(0.06f, yMin);
            child.anchorMax = new Vector2(0.94f, yMax);
            child.offsetMin = child.offsetMax = Vector2.zero;
        }

        static void PlaceCenter(RectTransform rt, Vector2 anchor)
        {
            rt.anchorMin = rt.anchorMax = anchor;
            rt.anchoredPosition = Vector2.zero;
        }

        static void ColorButtonCaption(Button btn, Color color)
        {
            var text = btn.GetComponentInChildren<Text>();
            if (text != null)
                text.color = color;
        }

        void OnListenAgain()
        {
            if (_state == SpeakUiState.Listening || _state == SpeakUiState.Processing)
                return;
            StartCoroutine(TapThen(_listenAgain.GetComponent<RectTransform>(), () =>
            {
                if (_audio != null)
                    _audio.PlayDemo(_demoClip, _levelId, string.IsNullOrEmpty(_clipId) ? "hi" : _clipId, null, canReplay: true, assess: false);
            }));
        }

        void OnMic()
        {
            if (_state == SpeakUiState.Processing)
                return;
            if (_state == SpeakUiState.Listening)
            {
                _stopListen = true;
                return;
            }

            StartCoroutine(TapThen(_micVisual != null ? _micVisual.rectTransform : (RectTransform)transform, () =>
            {
                if (_flow != null)
                    StopCoroutine(_flow);
                _flow = StartCoroutine(ListenFlow());
            }));
        }

        void OnRetry()
        {
            if (_state == SpeakUiState.Listening || _state == SpeakUiState.Processing)
                return;
            SetState(SpeakUiState.Idle);
            RaiseFeedback(FeedbackKind.Retry, "l1.rec.retry", "Try saying Hi again");
        }

        void OnSkip()
        {
            if (_recorder != null && _recorder.IsRecording)
                _recorder.StopCapture();
            RaiseFeedback(FeedbackKind.Skip, "l1.rec.skip", "Skipped speak-along without punishment");
            Skipped?.Invoke();
            Hide();
        }

        IEnumerator ListenFlow()
        {
            _stopListen = false;
            SetState(SpeakUiState.Listening);
            _recorder?.StartCapture(_levelId, _utteranceId);

            var elapsed = 0f;
            while (elapsed < DesignTokens.SpeakListenMax && _state == SpeakUiState.Listening)
            {
                elapsed += Time.unscaledDeltaTime;
                if (_stopListen && elapsed >= 0.8f)
                    break;
                if (elapsed >= 6.5f)
                    break;
                yield return null;
            }

            var path = _recorder != null ? _recorder.StopCapture() : string.Empty;
            var duration = _recorder != null ? _recorder.LastDuration : elapsed;

            SetState(SpeakUiState.Processing);
            var process = Mathf.Min(DesignTokens.SpeakProcessMax, 1.2f);
            yield return new WaitForSecondsRealtime(process);

            var result = _scorer != null
                ? _scorer.Score(path, duration)
                : SpeechScoreResult.StubSuccess(duration);

            if (duration < 0.45f)
            {
                FillStars(0);
                SetState(SpeakUiState.Retry);
                RaiseFeedback(FeedbackKind.Retry, "l1.rec.retry", "Let’s try Hi again");
            }
            else
            {
                FillStars(3);
                SetState(SpeakUiState.Success);
                RaiseFeedback(FeedbackKind.RecordStub, "l1.rec.success", result.Message);
                yield return UiMotion.Celebrate(_starLabels[1].rectTransform, DesignTokens.CelebrationSeconds);
                Succeeded?.Invoke();
                if (!BlockProgress)
                    Hide();
            }

            _flow = null;
        }

        void RaiseFeedback(string kind, string textKey, string message)
        {
            GameEventBus.RaiseFeedback(new FeedbackPayload
            {
                LevelId = _levelId,
                Kind = kind,
                TextKey = textKey,
                ScaffoldLevel = ScaffoldLevel,
                Message = message
            });
        }

        void SetState(SpeakUiState state)
        {
            _state = state;
            switch (state)
            {
                case SpeakUiState.Idle:
                    _status.text = BlockProgress
                        ? "Tap the mic and say  " + _sentence
                        : "Optional — say " + _sentence + "  想说也可以";
                    _status.color = DesignTokens.TextMuted;
                    _micVisual.color = DesignTokens.Primary;
                    _micCaption.text = "🎤";
                    FillStars(0);
                    break;
                case SpeakUiState.Listening:
                    _status.text = "Listening…  正在听你说";
                    _status.color = DesignTokens.Primary;
                    _micVisual.color = DesignTokens.SoftWarn;
                    _micCaption.text = "●";
                    break;
                case SpeakUiState.Processing:
                    _status.text = "Just a moment…  听完啦";
                    _status.color = DesignTokens.TextMuted;
                    _micVisual.color = DesignTokens.Primary;
                    break;
                case SpeakUiState.Success:
                    _status.text = "Nice speaking!  真好听";
                    _status.color = DesignTokens.Success;
                    _micVisual.color = DesignTokens.Success;
                    _micCaption.text = "★";
                    break;
                case SpeakUiState.Retry:
                    _status.text = "Let’s try Hi again  再试一次吧";
                    _status.color = DesignTokens.SoftWarn;
                    _micVisual.color = DesignTokens.SoftWarn;
                    break;
            }
        }

        void FillStars(int count)
        {
            if (_starLabels == null)
                return;
            for (var i = 0; i < _starLabels.Length; i++)
            {
                if (_starLabels[i] == null)
                    continue;
                _starLabels[i].text = i < count ? "★" : "☆";
                _starLabels[i].color = i < count ? DesignTokens.Success : DesignTokens.TextMuted;
            }
        }

        IEnumerator TapThen(RectTransform rt, Action action)
        {
            yield return UiMotion.Tap(rt);
            action?.Invoke();
        }
    }
}
