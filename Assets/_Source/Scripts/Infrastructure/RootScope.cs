using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ITCafe
{
    public class RootScope : LifetimeScope
    {
        [SerializeField] private Interactor _playerInteractor;
        [SerializeField] private ItemPicker _playerItemPicker;

        private CompositeDisposable _disposables;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponent<IItemPicker>(_playerItemPicker);
            builder.Register<PlayerContext>(Lifetime.Singleton);
        }

        protected override void Awake()
        {
            base.Awake();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            Container.Inject(_playerInteractor);

            _disposables = new();

            var playerContext = Container.Resolve<PlayerContext>();
            
            // _playerInteractor.OnItemInteracted.Subscribe(_playerItemPicker.TryPickUp).AddTo(_disposables);
            _playerItemPicker.IsHoldingItem.Subscribe(x => Debug.Log($"Holding item: x")).AddTo(_disposables);
            _playerItemPicker.CurrentItem.Subscribe(x => playerContext.CurrentItem = x).AddTo(_disposables);
        }

        protected override void OnDestroy()
        {
            _disposables.Dispose();
            _disposables = null;
            base.OnDestroy();
        }
    }
}