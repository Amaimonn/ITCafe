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
        [SerializeField] private Transform[] _clientSpawnPoints;
        [SerializeField] private Transform[] _clientOrderPoints;

        private Dictionary<Transform, bool> _orderAvailabilityMap;

        private CompositeDisposable _disposables;
        private CancellationToken _destroyToken;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<TableService>(new TableService(_clientSpawnPoints));
            builder.RegisterInstance<ClientCharacter[]>(_clientPrefabs)
                .As<IEnumerable<ClientCharacter>>()
                .As<ICollection<ClientCharacter>>()
                .As<IList<ClientCharacter>>()
                .AsSelf();
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
        }

        protected override void Awake()
        {
            base.Awake();
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _destroyToken = destroyCancellationToken;
            _orderAvailabilityMap = _clientOrderPoints.ToDictionary(x => x, _ => true);

            Container.Inject(_playerInteractor);

            _disposables = new();
            {
                _playerInteractor.CanInteract.Subscribe(x => _playerItemPicker.IsDroppingBlocked = x);
                _playerItemPicker.IsHoldingItem.Subscribe(x => Debug.Log($"Holding item: x"));
            }

            var clientsFactory = Container.Resolve<IFactory<ClientCharacter>>();
            var tableService = Container.Resolve<TableService>();
            RunClientsLifeCycle(clientsFactory, tableService, _destroyToken).Forget();
            // foreach (var spawnPoint in _clientSpawnPoints)
            //     clientsFactory.Create().transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        }

        private async UniTaskVoid RunClientsLifeCycle(IFactory<ClientCharacter> clientsFactory,
            TableService tableService, CancellationToken token)
        {
            try
            {
                await UniTask.Delay(1000, cancellationToken: token);
            }
            catch
            {
            }
            
            while (!token.IsCancellationRequested)
            {
                if (tableService.HasFreeTable)
                {
                    foreach (var orderTransform in _clientOrderPoints)
                    {
                        if (_orderAvailabilityMap.TryGetValue(orderTransform, out var isAvailable))
                        {
                            if (isAvailable)
                            {
                                var client = clientsFactory.Create();
                                client.transform.SetPositionAndRotation(orderTransform.position,
                                    orderTransform.rotation);
                                _orderAvailabilityMap[orderTransform] = false;
                                // TODO: Watch out for client subscription this time
                                Observable.Merge(client.OnLeft, client.OnOrdered)
                                    .Take(1)
                                    .Subscribe(x => _orderAvailabilityMap[orderTransform] = true);
                                break;
                            }
                        }
                    }
                }

                try
                {
                    await UniTask.Delay(2500, cancellationToken: token);
                }
                catch
                {
                }
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