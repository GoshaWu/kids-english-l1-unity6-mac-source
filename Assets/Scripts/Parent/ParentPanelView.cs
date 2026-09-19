using KidsEnglish.Audio;
using KidsEnglish.Core;
using KidsEnglish.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace KidsEnglish.Parent
{
    /// <summary>
    /// Read-only 会指认 / 敢跟读 lights. No accuracy, no ranking, no red X.
    /// Privacy contract: no child name, no face, no cloud voice, local JSON only.
    /// </summary>
    public sealed class ParentPanelView : MonoBehaviour
    {
        ParentModeController _controller;
        ProgressStore _progress;
        RectTransform _pinRoot;
        RectTransform _panelRoot;
        Text _progressLabel;
        Text _pinStatus;
        Toggle _sfxToggle;
        Toggle _speakToggle;
        readonly System.Text.StringBuilder _pinBuffer = new System.Text.StringBuilder();

        public static ParentPanelView Create(Transform canvas)
        {
            var go = new GameObject("ParentPanel", typeof(RectTransform), typeof(ParentPanelView));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(canvas, false);
            KidUi.Stretch(rt);
            var view = go.GetComponent<ParentPanelView>();
            view.Build(rt);
            view.Hide();
            return view;
        }

        public void Bind(ParentModeController controller, ProgressStore progress)
        {
            _controller = controller;
            _progress = progress;
            Refresh();
        }

        public void ShowPinPad()
        {
            gameObject.SetActive(true);
            _pinRoot.gameObject.SetActive(true);
            _panelRoot.gameObject.SetActive(false);
            _pinBuffer.Clear();
            if (_pinStatus != null)
                _pinStatus.text = "Grown-ups: enter PIN (default 0000)";
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _pinRoot.gameObject.SetActive(false);
            _panelRoot.gameObject.SetActive(true);
            Refresh();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        void Build(RectTransform root)
        {
            var dim = KidUi.Panel(root, "Dim", new Color(0.15f, 0.1f, 0.1f, 0.45f));
            KidUi.Stretch(dim);

            _pinRoot = KidUi.Panel(root, "PinPad", KidUi.Cloud);
            KidUi.Stretch(_pinRoot, 16, 16, 64, 64);

            var pinTitle = KidUi.Label(_pinRoot, "PinTitle", "Parent PIN  家长密码", 24, KidUi.Ink);
            pinTitle.rectTransform.anchorMin = new Vector2(0f, 0.78f);
            pinTitle.rectTransform.anchorMax = new Vector2(1f, 0.98f);
            pinTitle.rectTransform.offsetMin = pinTitle.rectTransform.offsetMax = Vector2.zero;

            _pinStatus = KidUi.Label(_pinRoot, "Status", "Enter PIN (default 0000)", 22, KidUi.Ink, FontStyle.Normal);
            _pinStatus.rectTransform.anchorMin = new Vector2(0f, 0.62f);
            _pinStatus.rectTransform.anchorMax = new Vector2(1f, 0.80f);
            _pinStatus.rectTransform.offsetMin = _pinStatus.rectTransform.offsetMax = Vector2.zero;

            var keys = new[] { "1", "2", "3", "0" };
            for (var i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                var col = i % 2;
                var row = i / 2;
                var btn = KidUi.BigButton(_pinRoot, "K" + key, key, KidUi.Sun, () => OnPinDigit(key), new Vector2(72, 72));
                var rt = btn.GetComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = new Vector2(0.32f + col * 0.36f, 0.48f - row * 0.22f);
                rt.anchoredPosition = Vector2.zero;
            }

            var cancel = KidUi.BigButton(_pinRoot, "CancelPin", "Close", KidUi.Sky, () =>
            {
                _controller?.Close();
                Hide();
            }, new Vector2(200, 64));
            var crt = cancel.GetComponent<RectTransform>();
            crt.anchorMin = crt.anchorMax = new Vector2(0.5f, 0.12f);
            crt.anchoredPosition = Vector2.zero;

            _panelRoot = KidUi.Panel(root, "Panel", KidUi.Cloud);
            KidUi.Stretch(_panelRoot, 12, 12, 36, 36);

            var title = KidUi.Label(_panelRoot, "Title", "Parent  家长", 40, KidUi.Ink);
            title.rectTransform.anchorMin = new Vector2(0f, 0.86f);
            title.rectTransform.anchorMax = new Vector2(1f, 1f);
            title.rectTransform.offsetMin = new Vector2(16, 0);
            title.rectTransform.offsetMax = new Vector2(-16, -8);

            var privacy = KidUi.Label(
                _panelRoot,
                "Privacy",
                "Privacy: no account, no face capture, voice stays on this device.\n不建档、不拍脸、录音只保存在本机。",
                20,
                KidUi.Ink,
                FontStyle.Normal,
                TextAnchor.UpperLeft);
            privacy.rectTransform.anchorMin = new Vector2(0f, 0.72f);
            privacy.rectTransform.anchorMax = new Vector2(1f, 0.88f);
            privacy.rectTransform.offsetMin = new Vector2(28, 0);
            privacy.rectTransform.offsetMax = new Vector2(-28, 0);

            _progressLabel = KidUi.Label(_panelRoot, "Progress", "Progress…", 18, KidUi.Ink, FontStyle.Normal, TextAnchor.UpperLeft);
            _progressLabel.rectTransform.anchorMin = new Vector2(0f, 0.34f);
            _progressLabel.rectTransform.anchorMax = new Vector2(1f, 0.74f);
            _progressLabel.rectTransform.offsetMin = new Vector2(24, 0);
            _progressLabel.rectTransform.offsetMax = new Vector2(-24, 0);

            _sfxToggle = MakeToggle(_panelRoot, "SFXToggle", "SFX  音效", new Vector2(0f, 28f), true, v => _progress?.SetSfx(v));
            _speakToggle = MakeToggle(_panelRoot, "SpeakToggle", "Speak-along  跟读", new Vector2(0f, -44f), true, v => _progress?.SetSpeakAlong(v));

            var del = KidUi.BigButton(_panelRoot, "DeleteVoice", "Delete voice", KidUi.Coral, () =>
            {
                LocalMicRecorder.DeleteLocalRecordings();
                Refresh();
            }, new Vector2(140, 56));
            var drt = del.GetComponent<RectTransform>();
            drt.anchorMin = drt.anchorMax = new Vector2(0.30f, 0.10f);
            drt.anchoredPosition = Vector2.zero;

            var close = KidUi.BigButton(_panelRoot, "Close", "Done", KidUi.Mint, () =>
            {
                _controller?.Close();
                Hide();
            }, new Vector2(120, 56));
            var closeRt = close.GetComponent<RectTransform>();
            closeRt.anchorMin = closeRt.anchorMax = new Vector2(0.72f, 0.10f);
            closeRt.anchoredPosition = Vector2.zero;
        }

        Toggle MakeToggle(Transform parent, string name, string label, Vector2 pos, bool on, UnityEngine.Events.UnityAction<bool> changed)
        {
            var holder = KidUi.Panel(parent, name, new Color(0.98f, 0.94f, 0.88f));
            holder.sizeDelta = new Vector2(280, 56);
            holder.anchorMin = holder.anchorMax = new Vector2(0.5f, 0.32f);
            holder.anchoredPosition = pos;
            var caption = KidUi.Label(holder, "L", label, 22, KidUi.Ink, FontStyle.Bold, TextAnchor.MiddleLeft);
            caption.rectTransform.offsetMin = new Vector2(16, 0);

            var togGo = new GameObject("Toggle", typeof(RectTransform), typeof(Toggle), typeof(Image));
            var trt = togGo.GetComponent<RectTransform>();
            trt.SetParent(holder, false);
            trt.anchorMin = trt.anchorMax = new Vector2(1f, 0.5f);
            trt.pivot = new Vector2(1f, 0.5f);
            trt.anchoredPosition = new Vector2(-18, 0);
            trt.sizeDelta = new Vector2(40, 40);
            var bg = togGo.GetComponent<Image>();
            bg.color = KidUi.Sun;
            var toggle = togGo.GetComponent<Toggle>();
            toggle.targetGraphic = bg;
            toggle.isOn = on;
            toggle.onValueChanged.AddListener(changed);
            return toggle;
        }

        void OnPinDigit(string digit)
        {
            _pinBuffer.Append(digit);
            if (_pinStatus != null)
                _pinStatus.text = new string('•', _pinBuffer.Length);
            if (_pinBuffer.Length < 4)
                return;

            var pin = _pinBuffer.ToString();
            _pinBuffer.Clear();
            if (_controller != null && _controller.TryOpenWithPin(pin))
                return;

            if (_pinStatus != null)
                _pinStatus.text = "Try again";
        }

        void Refresh()
        {
            if (_progress == null || _progressLabel == null)
                return;

            var identify = _progress.Data.identify ?? new IdentifyLights();
            var speak = _progress.Data.speakAlong ?? new SpeakLights();

            _progressLabel.text =
                Line(ParentMetrics.IdentifyTitleKey, "会指认  Can point") + "\n" +
                LightLine("Hi", ParentMetrics.IdentifyHiKey, identify.hi) + "  " +
                LightLine("Friend", ParentMetrics.IdentifyFriendKey, identify.friend) + "  " +
                LightLine("Me", ParentMetrics.IdentifyMeKey, identify.me) + "\n" +
                LightLine("You", ParentMetrics.IdentifyYouKey, identify.you) + "  " +
                LightLine("Happy", ParentMetrics.IdentifyHappyKey, identify.happy) + "  " +
                LightLine("Sad", ParentMetrics.IdentifySadKey, identify.sad) + "\n" +
                LightLine("Come play", ParentMetrics.IdentifyPlayComeKey, identify.playCome) + "  " +
                LightLine("Bye wave", ParentMetrics.IdentifyByeWaveKey, identify.byeWave) + "\n\n" +
                Line(ParentMetrics.SpeakTitleKey, "敢跟读  Willing to speak") + "\n" +
                LightLine("Hi!", ParentMetrics.SpeakHiKey, speak.hi) + "  " +
                LightLine("Bye!", ParentMetrics.SpeakByeKey, speak.bye) + "\n" +
                LightLine("Come play!", ParentMetrics.SpeakComePlayKey, speak.comePlay) + "  optional";

            if (_sfxToggle != null)
                _sfxToggle.SetIsOnWithoutNotify(_progress.Data.sfxEnabled);
            if (_speakToggle != null)
                _speakToggle.SetIsOnWithoutNotify(_progress.Data.speakAlongEnabled);
        }

        static string Line(string textKey, string label)
        {
            return label + "  [" + textKey + "]";
        }

        static string LightLine(string label, string textKey, bool on)
        {
            return ParentMetrics.Glyph(on) + " " + label + " [" + textKey + "]";
        }
    }
}
