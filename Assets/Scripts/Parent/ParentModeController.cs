using KidsEnglish.Audio;
using KidsEnglish.Progress;
using UnityEngine;

namespace KidsEnglish.Parent
{
    /// <summary>
    /// Grown-up gate. Does not collect a child profile.
    /// Default PIN is 0000 for the prototype — change it in the parent panel.
    /// </summary>
    public sealed class ParentModeController : MonoBehaviour
    {
        public bool IsOpen { get; private set; }

        ProgressStore _progress;
        ParentPanelView _view;

        public void Bind(ProgressStore progress, ParentPanelView view)
        {
            _progress = progress;
            _view = view;
            if (_view != null)
                _view.Bind(this, progress);
        }

        public void OpenFromLongPress()
        {
            Open();
        }

        public bool TryOpenWithPin(string pin)
        {
            var expected = _progress != null ? _progress.Data.parentPin : "0000";
            if (string.IsNullOrEmpty(expected))
                expected = "0000";
            if (pin != expected)
                return false;
            Open();
            return true;
        }

        public void Open()
        {
            IsOpen = true;
            if (_view != null)
                _view.Show();
        }

        public void Close()
        {
            IsOpen = false;
            if (_view != null)
                _view.Hide();
        }

        public void DeleteLocalVoice()
        {
            LocalMicRecorder.DeleteLocalRecordings();
        }
    }
}
