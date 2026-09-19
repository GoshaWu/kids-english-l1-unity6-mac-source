using KidsEnglish.Core;
using KidsEnglish.Progress;
using UnityEngine;

namespace KidsEnglish.Audio
{
    /// <summary>
    /// Plays lesson demo clips and light SFX.
    /// Interrupt policy: when a choice starts (or record starts), the demo stops immediately
    /// so two voices never overlap for ages 4–6.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioPlayer : MonoBehaviour
    {
        AudioSource _demo;
        AudioSource _sfx;
        ProgressStore _progress;
        AudioClip _currentDemo;
        string _currentClipId;
        string _currentTargetId;
        bool _canReplay;
        bool _assess;
        string _currentLevelId;

        public bool IsDemoPlaying => _demo != null && _demo.isPlaying;

        public void BindProgress(ProgressStore progress)
        {
            _progress = progress;
        }

        void Awake()
        {
            var sources = GetComponents<AudioSource>();
            _demo = sources.Length > 0 ? sources[0] : gameObject.AddComponent<AudioSource>();
            _demo.playOnAwake = false;
            _demo.loop = false;
            _demo.spatialBlend = 0f;

            _sfx = gameObject.AddComponent<AudioSource>();
            _sfx.playOnAwake = false;
            _sfx.loop = false;
            _sfx.spatialBlend = 0f;
            _sfx.volume = 0.7f;

            GameEventBus.OnChoice += OnChoiceInterrupt;
            GameEventBus.OnRecordStart += OnRecordInterrupt;
        }

        void OnDestroy()
        {
            GameEventBus.OnChoice -= OnChoiceInterrupt;
            GameEventBus.OnRecordStart -= OnRecordInterrupt;
        }

        public void PlayDemo(AudioClip clip, string levelId, string clipId, string targetId = null, bool canReplay = true, bool assess = false)
        {
            if (clip == null)
            {
                clip = WavUtility.MakeTone("demo_beep", new[] { 261.63f, 329.63f, 392f }, 1.15f);
                Debug.LogWarning("[KidsEnglish] Demo clip missing — playing placeholder arpeggio. Drop a wav on key " + clipId);
            }

            _currentDemo = clip;
            _currentClipId = clipId;
            _currentTargetId = targetId;
            _canReplay = canReplay;
            _assess = assess;
            _currentLevelId = levelId;
            _demo.clip = clip;
            _demo.volume = 1f;
            _demo.Play();

            GameEventBus.RaisePlayDemo(new PlayDemoPayload
            {
                LevelId = levelId,
                ClipId = clipId,
                TargetId = targetId,
                CanReplay = canReplay,
                Assess = assess
            });
        }

        public void PauseDemo()
        {
            if (_demo.isPlaying)
                _demo.Pause();
        }

        public void ReplayDemo(string levelId)
        {
            if (_currentDemo == null || !_canReplay)
                return;

            PlayDemo(_currentDemo, levelId ?? _currentLevelId, _currentClipId, _currentTargetId, _canReplay, _assess);
        }

        public void StopDemo()
        {
            if (_demo != null)
                _demo.Stop();
        }

        public void PlaySfx(AudioClip clip)
        {
            if (_progress != null && !_progress.Data.sfxEnabled)
                return;
            if (clip == null)
                return;
            _sfx.PlayOneShot(clip);
        }

        void OnChoiceInterrupt(ChoicePayload _)
        {
            StopDemo();
        }

        void OnRecordInterrupt(RecordStartPayload _)
        {
            StopDemo();
        }
    }
}
