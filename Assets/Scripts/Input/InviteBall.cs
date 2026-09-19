using System;
using System.Collections;
using KidsEnglish.Core;
using KidsEnglish.Levels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KidsEnglish.KidInput
{
    /// <summary>
    /// hit_ball 80×80: drag with snap 64px to drop_fox, or tap-to-send. Miss bounces home.
    /// </summary>
    public sealed class InviteBall : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public event Action<string> DroppedOn;
        public event Action Tapped;

        RectTransform _rt;
        RectTransform _homeParent;
        Canvas _canvas;
        Image _hit;
        Image _visual;
        Text _label;
        CharacterHotspot _snapTarget;
        bool _dragEnabled;
        bool _dragging;
        bool _tapHintMode;
        bool _moved;
        bool _raisedTap;
        Coroutine _pulse;

        public RectTransform Rect => _rt;
        public bool TapHintMode => _tapHintMode;

        public static InviteBall Create(Transform parent)
        {
            var hit = L1HelloFriends.HitSizeBall;
            var visual = 56f;
            var root = KidUi.Panel(parent, L1HelloFriends.HitBall, new Color(1f, 1f, 1f, 0.02f));
            root.anchorMin = root.anchorMax = L1HelloFriends.ParkBall;
            root.pivot = new Vector2(0.5f, 0.5f);
            root.sizeDelta = hit;

            var face = KidUi.Panel(root, L1HelloFriends.PropBall, new Color(0.93f, 0.33f, 0.31f, 1f));
            face.anchorMin = face.anchorMax = new Vector2(0.5f, 0.5f);
            face.sizeDelta = new Vector2(visual, visual);
            face.GetComponent<Image>().raycastTarget = false;

            var ball = root.gameObject.AddComponent<InviteBall>();
            ball._rt = root;
            ball._homeParent = parent as RectTransform;
            ball._hit = root.GetComponent<Image>();
            ball._visual = face.GetComponent<Image>();
            ball._label = KidUi.Label(root, "Label", "⚽ Ball  球", DesignTokens.FontMinSp, DesignTokens.Text);
            ball._label.rectTransform.anchorMin = new Vector2(-0.2f, -0.42f);
            ball._label.rectTransform.anchorMax = new Vector2(1.2f, 0.02f);
            ball._label.rectTransform.offsetMin = ball._label.rectTransform.offsetMax = Vector2.zero;
            ball._canvas = parent.GetComponentInParent<Canvas>();
            ball.SetDraggingEnabled(false);
            root.gameObject.SetActive(false);
            return ball;
        }

        public void Bind(CharacterHotspot snapTarget)
        {
            _snapTarget = snapTarget;
        }

        public void SetPortrait(Sprite sprite)
        {
            if (_visual == null || sprite == null)
                return;
            _visual.sprite = sprite;
            _visual.color = Color.white;
            _visual.preserveAspect = true;
        }

        public void SetTapHintMode(bool on)
        {
            _tapHintMode = on;
        }

        public void SetDraggingEnabled(bool value)
        {
            _dragEnabled = value;
            if (_hit != null)
                _hit.raycastTarget = value;
            if (!value && _dragging)
                SnapHome();
        }

        public void Show(bool on)
        {
            gameObject.SetActive(on);
            if (on)
                SnapHome();
        }

        public void ApplyScaffold(int scaffold, bool hintPulse)
        {
            if (_label != null)
            {
                _label.gameObject.SetActive(scaffold <= L1Scaffold.Reduced || _tapHintMode);
                if (_tapHintMode)
                    _label.text = "Tap  点一点";
                else
                    _label.text = scaffold <= L1Scaffold.Full ? "⚽ Ball  球" : "Ball";
            }

            StopPulse();
            if ((hintPulse || _tapHintMode) && gameObject.activeSelf)
                _pulse = StartCoroutine(UiMotion.Pulse(_rt, () => true));
            else if (_rt != null)
                _rt.localScale = Vector3.one;
        }

        public void SnapHome()
        {
            _dragging = false;
            if (_homeParent != null && _rt.parent != _homeParent)
                _rt.SetParent(_homeParent, false);
            _rt.anchorMin = _rt.anchorMax = L1HelloFriends.ParkBall;
            _rt.anchoredPosition = Vector2.zero;
            if (_visual != null)
                _visual.rectTransform.localScale = Vector3.one;
        }

        public IEnumerator FlyTo(RectTransform target)
        {
            if (target == null || _rt == null)
                yield break;

            _rt.SetAsLastSibling();
            var from = _rt.position;
            var to = target.position;
            var t = 0f;
            const float dur = 0.18f;
            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                _rt.position = Vector3.Lerp(from, to, Mathf.Clamp01(t / dur));
                yield return null;
            }

            _rt.position = to;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!_dragEnabled)
                return;
            _dragging = true;
            _moved = false;
            _raisedTap = false;
            _rt.SetAsLastSibling();
            StopPulse();
            if (_visual != null)
                _visual.rectTransform.localScale = Vector3.one * 1.06f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_dragging)
                return;
            var scale = _canvas != null ? _canvas.scaleFactor : 1f;
            _rt.anchoredPosition += eventData.delta / Mathf.Max(0.01f, scale);
            if (eventData.delta.sqrMagnitude > 1f)
                _moved = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_dragging)
                return;
            _dragging = false;
            if (_visual != null)
                _visual.rectTransform.localScale = Vector3.one;

            if (!_moved)
            {
                _raisedTap = true;
                Tapped?.Invoke();
                return;
            }

            DroppedOn?.Invoke(ResolveDrop(eventData.position));
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_dragEnabled || _dragging || _moved || _raisedTap)
                return;
            Tapped?.Invoke();
        }

        string ResolveDrop(Vector2 screen)
        {
            if (_snapTarget == null || !_snapTarget.gameObject.activeInHierarchy)
                return null;

            var dist = _snapTarget.DistanceDp(_rt, _canvas);
            if (dist <= L1HelloFriends.SnapRadiusPx)
                return _snapTarget.OptionId;
            if (_snapTarget.ContainsScreenPoint(screen))
                return _snapTarget.OptionId;
            return null;
        }

        public IEnumerator ShakeHome()
        {
            yield return UiMotion.Shake(_rt);
            SnapHome();
        }

        void StopPulse()
        {
            if (_pulse == null)
                return;
            StopCoroutine(_pulse);
            _pulse = null;
            if (_rt != null)
                _rt.localScale = Vector3.one;
        }

        void OnDisable()
        {
            StopPulse();
        }
    }
}
