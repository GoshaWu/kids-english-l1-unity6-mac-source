using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KidsEnglish.Core;
using KidsEnglish.Levels;

namespace KidsEnglish.KidInput
{
    /// <summary>
    /// Parent lock 🔒. Long-press opens parent mode. Visual ≥64dp; hitSlop may exceed bounds.
    /// </summary>
    public sealed class ParentEntryButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public float LongPressSeconds = 1.6f;

        public event Action LongPressed;
        public event Action Tapped;

        bool _held;
        float _downAt;
        bool _firedLong;

        public static ParentEntryButton Create(Transform parent)
        {
            var visual = DesignTokens.ParentLockDp;
            var slop = DesignTokens.HitSlopDp;
            var go = new GameObject(L1HelloFriends.UiParentLock, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-8f, -8f);
            rt.sizeDelta = new Vector2(visual + slop * 2f, visual + slop * 2f);
            go.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.02f);

            var face = KidUi.Panel(rt, "Visual", DesignTokens.Card);
            face.anchorMin = face.anchorMax = new Vector2(0.5f, 0.5f);
            face.sizeDelta = new Vector2(visual, visual);
            face.GetComponent<Image>().raycastTarget = false;
            KidUi.Label(face, "Lock", "🔒", 28, DesignTokens.Text);

            return go.AddComponent<ParentEntryButton>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _held = true;
            _firedLong = false;
            _downAt = Time.unscaledTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_held && !_firedLong && Time.unscaledTime - _downAt < LongPressSeconds)
                Tapped?.Invoke();
            _held = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _held = false;
        }

        void Update()
        {
            if (!_held || _firedLong)
                return;
            if (Time.unscaledTime - _downAt >= LongPressSeconds)
            {
                _firedLong = true;
                _held = false;
                LongPressed?.Invoke();
            }
        }
    }
}
