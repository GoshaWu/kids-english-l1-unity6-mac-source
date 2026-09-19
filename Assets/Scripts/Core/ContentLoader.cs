using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace KidsEnglish.Core
{
    /// <summary>
    /// Loads Addressables first, then Resources, then a caller-supplied fallback.
    /// Safe if Addressables groups have not been generated yet (first Editor open).
    /// </summary>
    public static class ContentLoader
    {
        public static IEnumerator LoadClip(string addressableKey, string resourcesPath, Action<AudioClip> onLoaded)
        {
            AudioClip clip = null;

            var handle = default(AsyncOperationHandle<AudioClip>);
            var started = false;
            try
            {
                handle = Addressables.LoadAssetAsync<AudioClip>(addressableKey);
                started = true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[KidsEnglish] Addressables audio '{addressableKey}' not ready: {ex.Message}");
            }

            if (started)
            {
                yield return handle;
                if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
                    clip = handle.Result;
            }

            if (clip == null && !string.IsNullOrEmpty(resourcesPath))
                clip = Resources.Load<AudioClip>(resourcesPath);

            onLoaded?.Invoke(clip);
        }

        public static IEnumerator LoadSprite(string addressableKey, string resourcesPath, Action<Sprite> onLoaded)
        {
            Sprite sprite = null;

            var spriteHandle = default(AsyncOperationHandle<Sprite>);
            var started = false;
            try
            {
                spriteHandle = Addressables.LoadAssetAsync<Sprite>(addressableKey);
                started = true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[KidsEnglish] Addressables sprite '{addressableKey}' not ready: {ex.Message}");
            }

            if (started)
            {
                yield return spriteHandle;
                if (spriteHandle.IsValid() && spriteHandle.Status == AsyncOperationStatus.Succeeded)
                    sprite = spriteHandle.Result;
            }

            if (sprite == null)
            {
                var texHandle = default(AsyncOperationHandle<Texture2D>);
                var texStarted = false;
                try
                {
                    texHandle = Addressables.LoadAssetAsync<Texture2D>(addressableKey);
                    texStarted = true;
                }
                catch (Exception)
                {
                    texStarted = false;
                }

                if (texStarted)
                {
                    yield return texHandle;
                    if (texHandle.IsValid() && texHandle.Status == AsyncOperationStatus.Succeeded)
                        sprite = SpriteFromTexture(texHandle.Result);
                }
            }

            if (sprite == null && !string.IsNullOrEmpty(resourcesPath))
            {
                sprite = Resources.Load<Sprite>(resourcesPath);
                if (sprite == null)
                    sprite = SpriteFromTexture(Resources.Load<Texture2D>(resourcesPath));
            }

            onLoaded?.Invoke(sprite);
        }

        static Sprite SpriteFromTexture(Texture2D tex)
        {
            if (tex == null)
                return null;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        }

        /// <summary>Soft colored circle used until art is dropped into Addressables.</summary>
        public static Sprite MakeColorSprite(Color color, int size = 128)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            var pixels = new Color[size * size];
            var r = size * 0.5f - 1f;
            var cx = size * 0.5f;
            var cy = size * 0.5f;
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var dx = x + 0.5f - cx;
                    var dy = y + 0.5f - cy;
                    pixels[y * size + x] = dx * dx + dy * dy <= r * r ? color : Color.clear;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
