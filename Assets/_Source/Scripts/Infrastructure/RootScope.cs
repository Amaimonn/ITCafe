using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using ITCafe.Player;
using ITCafe.Solutions;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ITCafe
{
    public class RootScope : LifetimeScope
    {
        [Header("Player")]
        [SerializeField] private Interactor _playerInteractor;
        [SerializeField] private ItemPicker _playerItemPicker;

        [Header("Clients"), Space(4)]
        [SerializeField] private ClientCharacter _clientPrefab;
        [SerializeField] private ItemInfoSO[] _itemInfoConfigs;

        private CompositeDisposable _disposables;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent<IItemPicker>(_playerItemPicker);
            builder.Register<PlayerContext>(Lifetime.Singleton);
            builder.Register<InputService>(Lifetime.Singleton);
            builder.Register<IFactory<ClientCharacter>>(x => new ClientsFactory(_itemInfoConfigs, _clientPrefab),
                Lifetime.Singleton);
        }

        protected override void Awake()
        {
            base.Awake();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            Container.Inject(_playerInteractor);

            _disposables = new();
            {
                // Нельзя выбрасывать предмет без соответствия логике взаимодействия
                _playerInteractor.CanInteract.Subscribe(x => _playerItemPicker.IsDroppingBlocked = x);
                _playerItemPicker.IsHoldingItem.Subscribe(x => Debug.Log($"Holding item: x"));
            }
        }

        protected override void OnDestroy()
        {
            _disposables.Dispose();
            _disposables = null;
            base.OnDestroy();
        }
    }
}