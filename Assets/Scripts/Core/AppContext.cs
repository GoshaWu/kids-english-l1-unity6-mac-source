using KidsEnglish.Audio;
using KidsEnglish.Parent;
using KidsEnglish.Progress;
using UnityEngine;

namespace KidsEnglish.Core
{
    /// <summary>
    /// Service locator spawned by Bootstrap. Lives across scene loads.
    /// Child data minimization: this object holds no name, photo, or account id.
    /// </summary>
    public sealed class AppContext : MonoBehaviour
    {
        public static AppContext Instance { get; private set; }

        public AudioPlayer Audio { get; private set; }
        public ProgressStore Progress { get; private set; }
        public LocalMicRecorder Recorder { get; private set; }
        public ISpeechScorer Scorer { get; private set; }
        public ParentModeController Parent { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            Instance = null;
            GameEventBus.ClearAll();
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Audio = GetComponent<AudioPlayer>() ?? gameObject.AddComponent<AudioPlayer>();
            Progress = GetComponent<ProgressStore>() ?? gameObject.AddComponent<ProgressStore>();
            Recorder = GetComponent<LocalMicRecorder>() ?? gameObject.AddComponent<LocalMicRecorder>();
            Parent = GetComponent<ParentModeController>() ?? gameObject.AddComponent<ParentModeController>();
            Scorer = new StubSpeechScorer();

            Progress.Load();
            Audio.BindProgress(Progress);
        }
    }
}
