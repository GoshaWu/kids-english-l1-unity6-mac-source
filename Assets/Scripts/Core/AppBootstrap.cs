using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KidsEnglish.Core
{
    /// <summary>
    /// First scene entry. Ensures services exist, then loads HelloFriends.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public sealed class AppBootstrap : MonoBehaviour
    {
        public const string HelloFriendsScene = "HelloFriends";

        [SerializeField] bool loadHelloFriends = true;

        void Awake()
        {
            EnsureCamera();
            EnsureEventSystem();
            EnsureCanvas();
            if (AppContext.Instance == null)
            {
                var services = new GameObject("AppServices");
                services.AddComponent<AppContext>();
            }
        }

        void Start()
        {
            if (!loadHelloFriends)
                return;

            if (SceneManager.GetActiveScene().name == HelloFriendsScene)
                return;

            SceneManager.LoadScene(HelloFriendsScene);
        }

        static void EnsureCamera()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera");
                cam = go.AddComponent<Camera>();
                go.tag = "MainCamera";
                go.AddComponent<AudioListener>();
            }

            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.nearClipPlane = -10f;
            cam.farClipPlane = 10f;
            cam.backgroundColor = DesignTokens.Bg;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.GetUniversalAdditionalCameraData();
        }

        static void EnsureEventSystem()
        {
            if (FindAnyObjectByType<EventSystem>() != null)
                return;

            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            DontDestroyOnLoad(go);
        }

        static void EnsureCanvas()
        {
            if (FindAnyObjectByType<Canvas>() != null)
                return;

            var go = new GameObject("BootstrapCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>();
            KidUi.ApplyPortraitCanvas(scaler);
        }
    }
}
