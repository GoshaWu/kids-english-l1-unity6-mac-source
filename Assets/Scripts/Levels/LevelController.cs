using System;
using KidsEnglish.Core;
using UnityEngine;

namespace KidsEnglish.Levels
{
    /// <summary>
    /// Per-level event surface. The bus uses the same five names; this class also exposes C# events
    /// so a scene can wire Inspector listeners later.
    /// </summary>
    public class LevelController : MonoBehaviour
    {
        public event Action<PlayDemoPayload> OnPlayDemo;
        public event Action<ChoicePayload> OnChoice;
        public event Action<RecordStartPayload> OnRecordStart;
        public event Action<RecordStopPayload> OnRecordStop;
        public event Action<FeedbackPayload> OnFeedback;

        public string LevelId { get; protected set; }

        protected virtual void OnEnable()
        {
            GameEventBus.OnPlayDemo += ForwardPlayDemo;
            GameEventBus.OnChoice += ForwardChoice;
            GameEventBus.OnRecordStart += ForwardRecordStart;
            GameEventBus.OnRecordStop += ForwardRecordStop;
            GameEventBus.OnFeedback += ForwardFeedback;
        }

        protected virtual void OnDisable()
        {
            GameEventBus.OnPlayDemo -= ForwardPlayDemo;
            GameEventBus.OnChoice -= ForwardChoice;
            GameEventBus.OnRecordStart -= ForwardRecordStart;
            GameEventBus.OnRecordStop -= ForwardRecordStop;
            GameEventBus.OnFeedback -= ForwardFeedback;
        }

        void ForwardPlayDemo(PlayDemoPayload p)
        {
            if (p.LevelId == LevelId) OnPlayDemo?.Invoke(p);
        }

        void ForwardChoice(ChoicePayload p)
        {
            if (p.LevelId == LevelId) OnChoice?.Invoke(p);
        }

        void ForwardRecordStart(RecordStartPayload p)
        {
            if (p.LevelId == LevelId) OnRecordStart?.Invoke(p);
        }

        void ForwardRecordStop(RecordStopPayload p)
        {
            if (p.LevelId == LevelId) OnRecordStop?.Invoke(p);
        }

        void ForwardFeedback(FeedbackPayload p)
        {
            if (p.LevelId == LevelId) OnFeedback?.Invoke(p);
        }
    }
}
