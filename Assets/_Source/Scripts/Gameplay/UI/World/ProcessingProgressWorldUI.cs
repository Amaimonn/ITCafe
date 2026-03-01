using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Gameplay.UI.World
{
    public class ProcessingProgressWorldUI : MonoBehaviour
    {
        [SerializeField] private UIDocument _worldDocument;
        [SerializeField] private Transform _uiHolder;
        [SerializeField] private string _progressBarName = "ProgressBar";

        private Transform _lookAtTarget;
        private VisualElement _root;
        private VisualElement _progressBar;

        private void Awake()
        {
            _lookAtTarget = Camera.main.transform;
            _root = _worldDocument.rootVisualElement;
            _progressBar = _root.Q<VisualElement>(name: _progressBarName);
            
            SetProgressUI(0);
            Hide();
        }

#region MonoBehaviour
        private void Update()
        {
            _uiHolder.rotation = Quaternion.LookRotation(_uiHolder.position - _lookAtTarget.position);
        }
#endregion

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