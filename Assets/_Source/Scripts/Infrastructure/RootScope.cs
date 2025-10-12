using System.Collections.Generic;
using System.Linq;
using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using ITCafe.Gameplay.CafeBusiness;
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
        [SerializeField] private AllItemInfoSO _allItemsInfoSO;

        private CompositeDisposable _disposables;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<ClientCharacter>(_clientPrefab);
            builder.Register<OrderGenerator>(Lifetime.Singleton);
            builder.RegisterInstance<ItemInfoSO[]>(_allItemsInfoSO.AllInfo).AsSelf().As<IEnumerable<ItemInfoSO>>();
            builder.Register<Dictionary<int, ItemInfoSO>>(x =>
                _allItemsInfoSO.AllInfo.ToDictionary(y => y.ItemInfo.GetItemHash()), Lifetime.Singleton);
            builder.RegisterComponent<IItemPicker>(_playerItemPicker);
            builder.Register<PlayerContext>(Lifetime.Singleton);
            builder.Register<InputService>(Lifetime.Singleton);
            builder.Register<ClientsFactory>(Lifetime.Singleton).As<IFactory<ClientCharacter>>();
        }

        protected override void Awake()
        {
            base.Awake();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            Container.Inject(_playerInteractor);

            _disposables = new();
            {
                _playerInteractor.CanInteract.Subscribe(x => _playerItemPicker.IsDroppingBlocked = x);
                _playerItemPicker.IsHoldingItem.Subscribe(x => Debug.Log($"Holding item: x"));
            }

            var clientsFactory = Container.Resolve<IFactory<ClientCharacter>>();
            clientsFactory.Create().transform.SetPositionAndRotation(new Vector3(8f, 0, -4f), Quaternion.Euler(0, 90, 0));
            clientsFactory.Create().transform.SetPositionAndRotation(new Vector3(8f, 0, -3.3f), Quaternion.Euler(0, 90, 0));
            clientsFactory.Create().transform.SetPositionAndRotation(new Vector3(8f, 0, -2.6f), Quaternion.Euler(0, 90, 0));
        }

        protected override void OnDestroy()
        {
            _disposables.Dispose();
            _disposables = null;
            base.OnDestroy();
        }
    }
}