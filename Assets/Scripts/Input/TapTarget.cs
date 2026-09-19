using UnityEngine;
using UnityEngine.UI;

namespace KidsEnglish.KidInput
{
    /// <summary>
    /// Large tap target for 4–6 year olds. Choice id is the only payload (no personal data).
    /// </summary>
    public sealed class TapTarget : MonoBehaviour
    {
        public string ChoiceId { get; private set; }
        public bool IsCorrect { get; private set; }

        Button _button;
        System.Action<TapTarget> _onTapped;

        public void Bind(string choiceId, bool isCorrect, System.Action<TapTarget> onTapped)
        {
            ChoiceId = choiceId;
            IsCorrect = isCorrect;
            _onTapped = onTapped;
            _button = GetComponent<Button>() ?? gameObject.AddComponent<Button>();
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => _onTapped?.Invoke(this));
        }

        public void SetInteractable(bool value)
        {
            if (_button != null)
                _button.interactable = value;
        }
    }
}
