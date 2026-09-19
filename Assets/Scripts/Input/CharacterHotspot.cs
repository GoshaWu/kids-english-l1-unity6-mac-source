using System;
using KidsEnglish.Core;
using UnityEngine;
using UnityEngine.UI;

namespace KidsEnglish.KidInput
{
    /// <summary>
    /// Park-gate hotspot. Hit box is the exact logical-px size. GameObject name is the locked hit id.
    /// </summary>
    public sealed class CharacterHotspot : MonoBehaviour
    {
        public string OptionId { get; private set; }

        public event Action<CharacterHotspot> Tapped;

        public RectTransform Rect => (RectTransform)transform;
        public Sprite PortraitSprite => _portrait != null ? _portrait.sprite : null;

        Image _card;
        Image _portrait;
        Image _warn;
        Text _label;
        Button _button;
        string _labelFull;
        string _labelShort;
        Coroutine _pulse;

        public static CharacterHotspot Create(
            Transform parent,
            string optionId,
            string labelFull,
            string labelShort,
            Color portraitColor,
            Vector2 anchor,
            Sprite portrait)
        {
            var hit = DesignTokens.CardMinDp + DesignTokens.HitSlopDp * 2f;
            var root = KidUi.Panel(parent, "Hotspot_" + optionId, new Color(1f, 1f, 1f, 0.02f));
            root.anchorMin = root.anchorMax = anchor;
            root.pivot = new Vector2(0.5f, 0.5f);
            root.sizeDelta = new Vector2(hit, hit);

            var card = KidUi.Panel(root, "Card", DesignTokens.Card);
            card.anchorMin = card.anchorMax = new Vector2(0.5f, 0.5f);
            card.sizeDelta = new Vector2(DesignTokens.CardMinDp, DesignTokens.CardMinDp);
            card.GetComponent<Image>().raycastTarget = false;

            var portraitRt = KidUi.Panel(card, "Portrait", portraitColor);
            portraitRt.anchorMin = new Vector2(0.12f, 0.32f);
            portraitRt.anchorMax = new Vector2(0.88f, 0.92f);
            portraitRt.offsetMin = portraitRt.offsetMax = Vector2.zero;
            var portraitImg = portraitRt.GetComponent<Image>();
            portraitImg.raycastTarget = false;
            if (portrait != null)
            {
                portraitImg.sprite = portrait;
                portraitImg.color = Color.white;
                portraitImg.preserveAspect = true;
            }

            var warn = KidUi.Panel(card, "Warn", new Color(DesignTokens.SoftWarn.r, DesignTokens.SoftWarn.g, DesignTokens.SoftWarn.b, 0.45f));
            KidUi.Stretch(warn);
            warn.GetComponent<Image>().raycastTarget = false;
            warn.gameObject.SetActive(false);

            var hotspot = root.gameObject.AddComponent<CharacterHotspot>();
            hotspot.OptionId = optionId;
            hotspot._card = card.GetComponent<Image>();
            hotspot._portrait = portraitImg;
            hotspot._warn = warn.GetComponent<Image>();
            hotspot._labelFull = labelFull;
            hotspot._labelShort = labelShort;
            hotspot._label = KidUi.Label(card, "Label", labelFull, DesignTokens.FontMinSp, DesignTokens.Text);
            hotspot._label.rectTransform.anchorMin = new Vector2(0.04f, 0.02f);
            hotspot._label.rectTransform.anchorMax = new Vector2(0.96f, 0.34f);
            hotspot._label.rectTransform.offsetMin = hotspot._label.rectTransform.offsetMax = Vector2.zero;
            hotspot._button = root.gameObject.AddComponent<Button>();
            hotspot._button.targetGraphic = root.GetComponent<Image>();
            hotspot._button.onClick.AddListener(() => hotspot.Tapped?.Invoke(hotspot));
            hotspot.SetTappable(false);
            return hotspot;
        }

        /// <summary>
        /// Park-gate hotspot. Hit box is the exact logical-px size. GameObject name is the locked hit id.
        /// </summary>
        public static CharacterHotspot CreatePark(
            Transform parent,
            string objectName,
            string optionId,
            string visualName,
            string labelFull,
            string labelShort,
            Color portraitColor,
            Vector2 anchor,
            Vector2 hitSize,
            Sprite portrait)
        {
            var root = KidUi.Panel(parent, objectName, new Color(1f, 1f, 1f, 0.02f));
            root.anchorMin = root.anchorMax = anchor;
            root.pivot = new Vector2(0.5f, 0.5f);
            root.sizeDelta = hitSize;

            var card = KidUi.Panel(root, "Card", DesignTokens.Card);
            card.anchorMin = card.anchorMax = new Vector2(0.5f, 0.5f);
            card.sizeDelta = hitSize;
            card.GetComponent<Image>().raycastTarget = false;

            var portraitRt = KidUi.Panel(card, visualName, portraitColor);
            KidUi.Stretch(portraitRt, 6, 6, 18, 6);
            var portraitImg = portraitRt.GetComponent<Image>();
            portraitImg.raycastTarget = false;
            portraitImg.preserveAspect = true;
            if (portrait != null)
            {
                portraitImg.sprite = portrait;
                portraitImg.color = Color.white;
            }

            var warn = KidUi.Panel(card, "Warn", new Color(DesignTokens.SoftWarn.r, DesignTokens.SoftWarn.g, DesignTokens.SoftWarn.b, 0.45f));
            KidUi.Stretch(warn);
            warn.GetComponent<Image>().raycastTarget = false;
            warn.gameObject.SetActive(false);

            var hotspot = root.gameObject.AddComponent<CharacterHotspot>();
            hotspot.OptionId = optionId;
            hotspot._card = card.GetComponent<Image>();
            hotspot._portrait = portraitImg;
            hotspot._warn = warn.GetComponent<Image>();
            hotspot._labelFull = labelFull;
            hotspot._labelShort = labelShort;
            hotspot._label = KidUi.Label(card, "Label", labelFull, DesignTokens.FontMinSp, DesignTokens.Text);
            hotspot._label.rectTransform.anchorMin = new Vector2(0.04f, 0.00f);
            hotspot._label.rectTransform.anchorMax = new Vector2(0.96f, 0.28f);
            hotspot._label.rectTransform.offsetMin = hotspot._label.rectTransform.offsetMax = Vector2.zero;
            hotspot._button = root.gameObject.AddComponent<Button>();
            hotspot._button.targetGraphic = root.GetComponent<Image>();
            hotspot._button.onClick.AddListener(() => hotspot.Tapped?.Invoke(hotspot));
            hotspot.SetTappable(false);
            return hotspot;
        }

        public float DistanceDp(RectTransform other, Canvas canvas)
        {
            if (other == null)
                return float.MaxValue;
            var scale = canvas != null ? Mathf.Max(0.01f, canvas.scaleFactor) : 1f;
            return Vector3.Distance(Rect.position, other.position) / scale;
        }

        public void SetPortrait(Sprite sprite, Color fallback)
        {
            if (_portrait == null)
                return;
            if (sprite != null)
            {
                _portrait.sprite = sprite;
                _portrait.color = Color.white;
                _portrait.preserveAspect = true;
            }
            else
            {
                _portrait.sprite = null;
                _portrait.color = fallback;
            }
        }

        public void SetVisible(bool on)
        {
            if (!on)
                StopPulse();
            gameObject.SetActive(on);
        }

        public void SetAnchor(Vector2 anchor)
        {
            Rect.anchorMin = Rect.anchorMax = anchor;
        }

        public void SetTappable(bool value)
        {
            if (_button != null)
                _button.interactable = value;
            var img = GetComponent<Image>();
            if (img != null)
                img.raycastTarget = value;
        }

        public void ShowWarn(bool on)
        {
            if (_warn != null)
                _warn.gameObject.SetActive(on);
            if (_card != null && !on)
                _card.color = DesignTokens.Card;
        }

        public void ApplyScaffold(int scaffold, bool hintTarget)
        {
            if (_label != null)
            {
                if (scaffold <= L1Scaffold.Full)
                    _label.text = _labelFull;
                else if (scaffold == L1Scaffold.Reduced)
                    _label.text = _labelShort;
                else
                    _label.text = string.Empty;
            }

            StopPulse();
            if (hintTarget && scaffold <= L1Scaffold.Full)
                _pulse = StartCoroutine(UiMotion.Pulse(Rect, () => true));
            else
                Rect.localScale = Vector3.one;
        }

        public bool ContainsScreenPoint(Vector2 screen)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(Rect, screen, null);
        }

        public void StopPulse()
        {
            if (_pulse == null)
                return;
            StopCoroutine(_pulse);
            _pulse = null;
            Rect.localScale = Vector3.one;
        }

        void OnDisable()
        {
            StopPulse();
        }
    }

    public static class L1Scaffold
    {
        public const int Full = 1;
        public const int Reduced = 2;
        public const int Withdrawn = 3;
    }
}
