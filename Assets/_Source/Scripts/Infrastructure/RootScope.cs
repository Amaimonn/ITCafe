using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using ITCafe.Gameplay.CafeBusiness;
using ITCafe.Gameplay.UI.MVVM;
using ITCafe.Player;
using ITCafe.Solutions;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;
using Cursor = UnityEngine.Cursor;

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

        [Header("UI")]
        [SerializeField] private UIDocument _uiDocument;
        [SerializeField] private VisualTreeAsset _aimAsset;
        [SerializeField] private HUDView _hudView;

        private CompositeDisposable _disposables;
        private CancellationToken _destroyToken;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterUI(builder);

            builder.RegisterInstance<ClientCharacter[]>(_clientPrefabs)
                .As<IEnumerable<ClientCharacter>>()
                .As<ICollection<ClientCharacter>>()
                .As<IReadOnlyList<ClientCharacter>>()
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

            builder.Register<ClientsRunner>(Lifetime.Singleton);

            builder.Register<GameSessionRunner>(Lifetime.Singleton);

            builder.Register<WorkProgressService>(Lifetime.Singleton);
        }

        private void RegisterUI(IContainerBuilder builder)
        {
            builder.RegisterComponent<HUDView>(_hudView);
            builder.Register<HUDViewModel>(Lifetime.Singleton);
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
                _playerItemPicker.IsHoldingItem.Subscribe(x => Debug.Log($"Holding item: {x}"));
            }
            
            var progressService = Container.Resolve<WorkProgressService>();
            var hudViewModel = Container.Resolve<HUDViewModel>();
            progressService.OnOrderTaken.Subscribe(_ => hudViewModel.IncrementOrdersTaken());
            progressService.OnClientServed.Subscribe(_ => hudViewModel.IncrementOrdersCompleted());
            
            var sessionRunner = Container.Resolve<GameSessionRunner>();
            sessionRunner.RunSessionAsync(_destroyToken).Forget();


#if UNITY_EDITOR
            Observable.EveryUpdate().Where(_ => Keyboard.current.digit0Key.wasPressedThisFrame)
                .Take(1)
                .Subscribe(_ => sessionRunner.CompleteSession());
#endif
        }

        private void Start()
        {
            _uiDocument.rootVisualElement.Clear();
            
            var aimElement = _aimAsset.CloneTree();
            aimElement.pickingMode = PickingMode.Ignore;
            aimElement.style.position = Position.Absolute;
            aimElement.style.width = Length.Percent(100);
            aimElement.style.height = Length.Percent(100);
            _uiDocument.rootVisualElement.Add(aimElement);

            var hudViewModel = Container.Resolve<HUDViewModel>();
            var hudElement = _hudView.InitAndGetRoot();
            _hudView.Bind(hudViewModel);
            _uiDocument.rootVisualElement.Add(hudElement);
        }

        protected override void OnDestroy()
        {
            _disposables.Dispose();
            _disposables = null;
            base.OnDestroy();
        }
    }
}