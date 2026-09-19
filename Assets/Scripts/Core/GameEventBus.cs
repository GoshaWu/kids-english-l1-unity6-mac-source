using System;
using UnityEngine;

namespace KidsEnglish.Core
{
    /// <summary>
    /// Tiny static bus so 玩法 / 美术 / audio can listen without hard scene wiring.
    /// Event names are exact: OnPlayDemo, OnChoice, OnRecordStart, OnRecordStop, OnFeedback.
    /// </summary>
    public static class GameEventBus
    {
        public static event Action<PlayDemoPayload> OnPlayDemo;
        public static event Action<ChoicePayload> OnChoice;
        public static event Action<RecordStartPayload> OnRecordStart;
        public static event Action<RecordStopPayload> OnRecordStop;
        public static event Action<FeedbackPayload> OnFeedback;

        public static void RaisePlayDemo(PlayDemoPayload payload)
        {
            Debug.Log($"[KidsEnglish] OnPlayDemo clipId={payload.ClipId} targetId={payload.TargetId} canReplay={payload.CanReplay} assess={payload.Assess}");
            OnPlayDemo?.Invoke(payload);
        }

        public static void RaiseChoice(ChoicePayload payload)
        {
            Debug.Log($"[KidsEnglish] OnChoice choiceId={payload.ChoiceId} optionId={payload.OptionId} correct={payload.Correct}");
            OnChoice?.Invoke(payload);
        }

        public static void RaiseRecordStart(RecordStartPayload payload)
        {
            Debug.Log($"[KidsEnglish] OnRecordStart utteranceId={payload.UtteranceId}");
            OnRecordStart?.Invoke(payload);
        }

        public static void RaiseRecordStop(RecordStopPayload payload)
        {
            Debug.Log($"[KidsEnglish] OnRecordStop utteranceId={payload.UtteranceId} audioRef={payload.AudioRef} score={payload.Score:0.00}");
            OnRecordStop?.Invoke(payload);
        }

        public static void RaiseFeedback(FeedbackPayload payload)
        {
            Debug.Log($"[KidsEnglish] OnFeedback kind={payload.Kind} textKey={payload.TextKey} scaffoldLevel={payload.ScaffoldLevel}");
            OnFeedback?.Invoke(payload);
        }

        /// <summary>Call when leaving play mode or resetting a level so listeners do not leak.</summary>
        public static void ClearAll()
        {
            OnPlayDemo = null;
            OnChoice = null;
            OnRecordStart = null;
            OnRecordStop = null;
            OnFeedback = null;
        }
    }
}
