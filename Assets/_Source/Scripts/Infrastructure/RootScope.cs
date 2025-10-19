using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using ITCafe.Gameplay.CafeBusiness;
using ITCafe.Player;
using ITCafe.Solutions;
using R3;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private ClientCharacter[] _clientPrefabs;
        [SerializeField] private AllItemInfoSO _allItemsInfoSO;
        [SerializeField] private Transform[] _clientSeatPoints;
        [SerializeField] private Transform[] _clientOrderPoints;


        private CompositeDisposable _disposables;
        private CancellationToken _destroyToken;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<ClientCharacter[]>(_clientPrefabs)
                .As<IEnumerable<ClientCharacter>>()
                .As<ICollection<ClientCharacter>>()
                .As<IList<ClientCharacter>>()
                .AsSelf();
            
            builder.RegisterInstance<Transform[]>(_clientSeatPoints)
                .AsSelf()
                .As<IEnumerable<Transform>>()
                .Keyed(Constants.CLIENT_SEATS);
            
            builder.RegisterInstance<Transform[]>(_clientOrderPoints)
                .AsSelf()
                .As<IEnumerable<Transform>>()
                .Keyed(Constants.CLIENT_ORDER_PLACES);
            
            builder.Register<TableService>(Lifetime.Singleton);
            
            builder.Register<OrderGenerator>(Lifetime.Singleton);
            
            builder.RegisterInstance<ItemInfoSO[]>(_allItemsInfoSO.AllInfo)
                .AsSelf()
                .As<IEnumerable<ItemInfoSO>>();
            
            builder.Register<Dictionary<int, ItemInfoSO>>(x =>
                _allItemsInfoSO.AllInfo.ToDictionary(y => y.ItemInfo.GetItemHash()), Lifetime.Singleton);
            
            builder.RegisterComponent<IItemPicker>(_playerItemPicker);
            
            builder.Register<PlayerContext>(Lifetime.Singleton);
            
            builder.Register<InputService>(Lifetime.Singleton);
            
            builder.Register<ClientsFactory>(Lifetime.Singleton)
                .As<IFactory<ClientCharacter>>();
            
            builder.Register<CafeRunner>(Lifetime.Singleton);
        }

        protected override void Awake()
        {
            base.Awake();
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _destroyToken = destroyCancellationToken;

            Container.Inject(_playerInteractor);

            _disposables = new();
            {
                _playerInteractor.CanInteract.Subscribe(x => _playerItemPicker.IsDroppingBlocked = x);
                _playerItemPicker.IsHoldingItem.Subscribe(x => Debug.Log($"Holding item: x"));
            }

            var cafeRunner = Container.Resolve<CafeRunner>();
            cafeRunner.RunClientsLifeCycle(_destroyToken).Forget();
        }

        protected override void OnDestroy()
        {
            _disposables.Dispose();
            _disposables = null;
            base.OnDestroy();
        }
    }
}