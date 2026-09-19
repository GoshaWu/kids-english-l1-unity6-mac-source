using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KidsEnglish.Core
{
    /// <summary>
    /// Runtime UI helpers. Tokens in <see cref="DesignTokens"/>. Canvas is 390×844 portrait (1 unit = 1 logical px).
    /// </summary>
    public static class KidUi
    {
        public static Color Bg => DesignTokens.Bg;
        public static Color Primary => DesignTokens.Primary;
        public static Color Success => DesignTokens.Success;
        public static Color SoftWarn => DesignTokens.SoftWarn;
        public static Color Card => DesignTokens.Card;
        public static Color Ink => DesignTokens.Text;

        public static readonly Color Cream = DesignTokens.Bg;
        public static readonly Color Coral = DesignTokens.SoftWarn;
        public static readonly Color Sun = DesignTokens.SoftWarn;
        public static readonly Color Sky = DesignTokens.Primary;
        public static readonly Color Mint = DesignTokens.Success;
        public static readonly Color Leaf = DesignTokens.Success;
        public static readonly Color Cloud = DesignTokens.Card;

        public static Font DefaultFont
        {
            get
            {
                return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
        }

        public static Color Hex(string hex) => DesignTokens.Hex(hex);

        public static RectTransform Panel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            go.GetComponent<Image>().color = color;
            go.GetComponent<Image>().raycastTarget = true;
            return rt;
        }

        public static Text Label(Transform parent, string name, string value, int size, Color color, FontStyle style = FontStyle.Bold, TextAnchor anchor = TextAnchor.MiddleCenter)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            Stretch(rt);
            var text = go.GetComponent<Text>();
            text.font = DefaultFont;
            text.fontSize = Mathf.Max(DesignTokens.FontMinSp, size);
            text.fontStyle = style;
            text.color = color;
            text.alignment = anchor;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            text.text = value;
            return text;
        }

        public static Button BigButton(Transform parent, string name, string caption, Color color, UnityAction onClick, Vector2 size)
        {
            var visual = Mathf.Max(size.x, size.y);
            var hit = HitButton(parent, name, color, onClick, size, DesignTokens.HitSlopDp);
            var captionSize = Mathf.Max(DesignTokens.FontMinSp, Mathf.RoundToInt(visual * 0.22f));
            var label = Label(hit, "Caption", caption, captionSize, DesignTokens.Text);
            label.rectTransform.offsetMin = Vector2.zero;
            label.rectTransform.offsetMax = Vector2.zero;
            return hit.GetComponent<Button>();
        }

        public static RectTransform HitButton(Transform parent, string name, Color color, UnityAction onClick, Vector2 visualSize, float slop)
        {
            var hit = Panel(parent, name, Color.clear);
            hit.sizeDelta = visualSize + Vector2.one * (slop * 2f);
            var visual = Panel(hit, "Visual", color);
            visual.anchorMin = visual.anchorMax = new Vector2(0.5f, 0.5f);
            visual.sizeDelta = visualSize;
            visual.GetComponent<Image>().raycastTarget = false;
            var btn = hit.gameObject.AddComponent<Button>();
            btn.targetGraphic = hit.GetComponent<Image>();
            hit.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.01f);
            btn.onClick.AddListener(onClick);
            return hit;
        }

        public static void Stretch(RectTransform rt, float left = 0, float right = 0, float top = 0, float bottom = 0)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
        }

        public static void AnchorTop(RectTransform rt, float height, float left, float right, float top)
        {
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.anchoredPosition = new Vector2(0f, -top);
            rt.sizeDelta = new Vector2(-(left + right), height);
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
            rt.offsetMax = new Vector2(-right, -top);
        }

        public static CanvasScaler ApplyPortraitCanvas(CanvasScaler scaler)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = DesignTokens.PortraitReference;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            return scaler;
        }
    }
}
