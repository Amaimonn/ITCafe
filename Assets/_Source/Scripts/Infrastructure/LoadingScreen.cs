using System.Collections;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace ITCafe
{
    public class LoadingScreen : MonoBehaviour
    {
        public Observable<Unit> OnStarted => _onStarted;
        public Observable<Unit> OnFinished => _onFinished;
        public Observable<float> OverlayFillProgress => _overlayFillProgress;

        [SerializeField] private GameObject _loadingRoot;
        [SerializeField] private Image _overlayImage;
        [SerializeField] private GameObject _loadingStub;
        [SerializeField] private GameObject _loadingText;
        [SerializeField] private string _overlayProgressProperty;
        [SerializeField, Min(0.01f)] private float _overlayFillSeconds = 2f;

        private readonly Subject<Unit> _onStarted = new();
        private readonly Subject<Unit> _onFinished = new();
        private readonly ReactiveProperty<float> _overlayFillProgress = new(1);


#region MonoBehaviour
        private void Awake()
        {
            Hide();
        }
#endregion

        public IEnumerator ShowWithInstantCoroutine(bool isInstant)
        {
            if (isInstant)
                Show();
            else
                yield return ShowCoroutine();
        }
        
        public IEnumerator HideWithInstantCoroutine(bool isInstant)
        {
            if (isInstant)
                Hide();
            else
                yield return HideCoroutine();
        }
        
        public void Show()
        {
            _loadingRoot.SetActive(true);
            SetActiveTextSafe(true);
            SetActiveStubSafe(true);

            SetOverlayFillProgress(1);
            
            _onStarted.OnNext(Unit.Default);
        }

        public void Hide()
        {
            _loadingRoot.SetActive(false);
            SetActiveStubSafe(false);
            SetActiveTextSafe(false);
            
            SetOverlayFillProgress(0);
            
            _onFinished.OnNext(Unit.Default);
        }

        public IEnumerator ShowCoroutine()
        {
            _onStarted.OnNext(Unit.Default);
            _loadingRoot.SetActive(true);

            while (_overlayFillProgress.Value < 1)
            {
                var currentProgress = _overlayFillProgress.Value + Time.unscaledDeltaTime / _overlayFillSeconds;
                
                if (currentProgress > 1)
                    currentProgress = 1;
                
                SetOverlayFillProgress(currentProgress);
                yield return null;
            }

            SetActiveStubSafe(true);
            SetActiveTextSafe(true);
        }

        public IEnumerator HideCoroutine()
        {
            SetActiveStubSafe(false);
            SetActiveTextSafe(false);

            while (_overlayFillProgress.Value > 0)
            {
                var currentProgress = _overlayFillProgress.Value - Time.unscaledDeltaTime / _overlayFillSeconds;
                if (currentProgress < 0)
                    currentProgress = 0;
                SetOverlayFillProgress(currentProgress);
                yield return null;
            }

            _loadingRoot.SetActive(false);
            _onFinished.OnNext(Unit.Default);
        }

        private void SetOverlayFillProgress(float progress)
        {
            _overlayFillProgress.Value = progress;
            _overlayImage.material.SetFloat(_overlayProgressProperty, _overlayFillProgress.Value);
        }

        private void SetActiveTextSafe(bool isActive)
        {
            if (_loadingText != null)
                _loadingText.SetActive(isActive);
        }
        
        private void SetActiveStubSafe(bool isActive)
        {
            if (_loadingStub != null)
                _loadingStub.SetActive(isActive);
        }
    }
}