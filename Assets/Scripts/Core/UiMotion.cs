using System.Collections;
using UnityEngine;

namespace KidsEnglish.Core
{
    /// <summary>
    /// Wireframe motion: tap 120–180ms, celebration ≤1.2s, gentle shake (never a scare).
    /// </summary>
    public static class UiMotion
    {
        public static IEnumerator Tap(RectTransform rt)
        {
            if (rt == null)
                yield break;

            var start = rt.localScale;
            var pressed = start * 0.92f;
            var half = DesignTokens.TapSeconds * 0.5f;
            yield return ScaleTo(rt, pressed, half);
            yield return ScaleTo(rt, start, half);
        }

        public static IEnumerator Shake(RectTransform rt, float duration = 0.32f)
        {
            if (rt == null)
                yield break;

            var origin = rt.anchoredPosition;
            var t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                var wobble = Mathf.Sin(t * 42f) * 8f * (1f - t / duration);
                rt.anchoredPosition = origin + new Vector2(wobble, 0f);
                yield return null;
            }

            rt.anchoredPosition = origin;
        }

        public static IEnumerator Pulse(RectTransform rt, System.Func<bool> whileTrue)
        {
            if (rt == null)
                yield break;

            var baseScale = Vector3.one;
            while (whileTrue != null && whileTrue())
            {
                yield return ScaleTo(rt, baseScale * 1.06f, 0.45f);
                if (whileTrue != null && !whileTrue())
                    break;
                yield return ScaleTo(rt, baseScale, 0.45f);
            }

            rt.localScale = baseScale;
        }

        public static IEnumerator Celebrate(RectTransform star, float seconds = DesignTokens.CelebrationSeconds)
        {
            if (star == null)
                yield break;

            star.gameObject.SetActive(true);
            star.localScale = Vector3.zero;
            var t = 0f;
            var pop = Mathf.Min(0.35f, seconds * 0.35f);
            while (t < pop)
            {
                t += Time.unscaledDeltaTime;
                var k = Mathf.Clamp01(t / pop);
                star.localScale = Vector3.one * Mathf.SmoothStep(0f, 1.15f, k);
                yield return null;
            }

            star.localScale = Vector3.one;
            yield return new WaitForSecondsRealtime(Mathf.Max(0.05f, seconds - pop - 0.2f));
            yield return ScaleTo(star, Vector3.one, 0.2f);
        }

        public static IEnumerator Catch(RectTransform rt, float seconds = 0.45f)
        {
            if (rt == null)
                yield break;

            var start = rt.localScale;
            yield return ScaleTo(rt, start * 1.18f, seconds * 0.4f);
            yield return ScaleTo(rt, start, seconds * 0.6f);
        }

        public static IEnumerator Wave(RectTransform rt, float seconds = 1.05f)
        {
            if (rt == null)
                yield break;

            var origin = rt.localEulerAngles;
            var t = 0f;
            seconds = Mathf.Min(seconds, DesignTokens.CelebrationSeconds);
            while (t < seconds)
            {
                t += Time.unscaledDeltaTime;
                var angle = Mathf.Sin(t * 11f) * 16f;
                rt.localEulerAngles = new Vector3(0f, 0f, angle);
                yield return null;
            }

            rt.localEulerAngles = origin;
        }

        static IEnumerator ScaleTo(RectTransform rt, Vector3 target, float duration)
        {
            var from = rt.localScale;
            var t = 0f;
            duration = Mathf.Max(0.04f, duration);
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                rt.localScale = Vector3.Lerp(from, target, Mathf.Clamp01(t / duration));
                yield return null;
            }

            rt.localScale = target;
        }
    }
}
