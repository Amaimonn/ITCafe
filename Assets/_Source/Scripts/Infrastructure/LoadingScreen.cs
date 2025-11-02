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

        [SerializeField] private GameObject _loadingGameObject;
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

        public void Show(bool showText = true)
        {
            _loadingGameObject.SetActive(true);
            _loadingStub.SetActive(true);
            if (showText)
                _loadingText.SetActive(true);
            SetOverlayFillProgress(1);
            _onStarted.OnNext(Unit.Default);
        }

        public void Hide()
        {
            _loadingStub.SetActive(false);
            _loadingText.SetActive(false);
            _loadingGameObject.SetActive(false);
            SetOverlayFillProgress(0);
            _onFinished.OnNext(Unit.Default);
        }

        public IEnumerator ShowCoroutine()
        {
            _onStarted.OnNext(Unit.Default);
            _loadingGameObject.SetActive(true);

            while (_overlayFillProgress.Value < 1)
            {
                var currentProgress = _overlayFillProgress.Value + Time.deltaTime / _overlayFillSeconds;
                if (currentProgress > 1)
                    currentProgress = 1;
                SetOverlayFillProgress(currentProgress);
                yield return null;
            }

            _loadingStub.SetActive(true);
        }

        public IEnumerator HideCoroutine()
        {
            _loadingStub.SetActive(false);

            while (_overlayFillProgress.Value > 0)
            {
                var currentProgress = _overlayFillProgress.Value - Time.deltaTime / _overlayFillSeconds;
                if (currentProgress < 0)
                    currentProgress = 0;
                SetOverlayFillProgress(currentProgress);
                yield return null;
            }

            _loadingGameObject.SetActive(false);
            _onFinished.OnNext(Unit.Default);
        }

        private void SetOverlayFillProgress(float progress)
        {
            _overlayFillProgress.Value = progress;
            _overlayImage.material.SetFloat(_overlayProgressProperty, _overlayFillProgress.Value);
        }
    }
}