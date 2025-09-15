using R3;
using UnityEngine;

namespace ITCafe
{
    public class RootScope : MonoBehaviour
    {
        [SerializeField] private Interactor _playerInteractor;
        [SerializeField] private ItemPicker _playerItemPicker;

        private CompositeDisposable _disposables;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _disposables = new();
            _playerInteractor.OnItemInteracted.Subscribe(_playerItemPicker.TryPickUp).AddTo(_disposables);
            _playerItemPicker.IsHoldingItem.Subscribe(x => Debug.Log($"Holding item: x")).AddTo(_disposables);
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
            _disposables = null;
        }
    }
}