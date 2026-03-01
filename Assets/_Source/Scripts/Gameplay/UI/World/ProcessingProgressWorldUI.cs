using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Gameplay.UI.World
{
    public class ProcessingProgressWorldUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _worldDocument;
        [SerializeField] private string _progressBarName = "ProgressBar";

        private VisualElement _root;
        private VisualElement _progressBar;

        private void Awake()
        {
            _root = _worldDocument.rootVisualElement;
            _progressBar = _root.Q<VisualElement>(name: _progressBarName);
            
            SetProgressUI(0);
            Hide();
        }

        public void SetProgress(float remainingTimeNormalized)
        {
            var progressNormalized = Mathf.Clamp01(remainingTimeNormalized);
            SetProgressUI(progressNormalized);
        }

        public void Show()
        {
            _root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _root.style.display = DisplayStyle.None;
        }

        private void SetProgressUI(float progressNormalized)
        {
            _progressBar.style.width = Length.Percent(progressNormalized * 100f);
        }
    }
}