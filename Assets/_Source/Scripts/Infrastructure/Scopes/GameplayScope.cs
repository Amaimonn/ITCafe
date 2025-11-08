using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DevKit.Solutions;
using DevKit.UI.MVVM;
using DevKit.UI.MVVM.Bases;
using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using ITCafe.Gameplay.CafeBusiness;
using ITCafe.Gameplay.UI.MVVM;
using ITCafe.Player;
using R3;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;
using Cursor = UnityEngine.Cursor;
using Unit = R3.Unit;

namespace ITCafe
{
    public class GameplayScope : LifetimeScope
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
        [SerializeField] private AimView _aimViewPrefab;
        [SerializeField] private HUDView _hudViewPrefab;

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

        public Observable<GameplayExitContext> Boot(GameplayEnterContext gameplayEnterContext = null)
        {
            Build();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _destroyToken = destroyCancellationToken;

            Container.Inject(_playerInteractor);
            _playerInteractor.Init();

            _disposables = new();
            {
                _playerInteractor.CanInteract.Subscribe(x => _playerItemPicker.IsDroppingBlocked = x);
                _playerItemPicker.IsHoldingItem.Subscribe(x => Debug.Log($"Holding item: {x}"));
            }

            var sessionRunner = Container.Resolve<GameSessionRunner>();
            sessionRunner.RunSessionAsync(_destroyToken).Forget();

#if UNITY_EDITOR
            Observable.EveryUpdate().Where(_ => Keyboard.current.digit0Key.wasPressedThisFrame)
                .Take(1)
                .Subscribe(_ => sessionRunner.CompleteSession());
#endif

            InitUI();

            var exitSignal = new Subject<Unit>(); // untyped signal

            var mainMenuEnterContext = new MainMenuEnterContext();
            var gameplayExitContext = new GameplayExitContext(mainMenuEnterContext);
            var gameplayExitSignal = exitSignal.Select(_ => gameplayExitContext);

            return gameplayExitSignal;
        }

        private void RegisterUI(IContainerBuilder builder)
        {
            builder.RegisterInstance<HUDView>(_hudViewPrefab); // prefab registration
            builder.Register<HUDViewModel>(Lifetime.Singleton);
            builder.Register<LazyAttachBinder<HUDView, HUDViewModel>>(Lifetime.Singleton)
                .As<IViewBinder<HUDView>>();
            builder.Register<Func<HUDViewModel>>(x => () =>
            {
                var hudViewModel = x.Resolve<HUDViewModel>();
                var progressService = Container.Resolve<WorkProgressService>();
                progressService.OnOrderTaken.Subscribe(_ => hudViewModel.IncrementOrdersTaken());
                progressService.OnClientServed.Subscribe(_ => hudViewModel.IncrementOrdersCompleted());
                return hudViewModel;
            }, Lifetime.Singleton);
            
            builder.RegisterInstance<AimView>(_aimViewPrefab); // prefab registration
            builder.Register<AimViewModel>(Lifetime.Singleton);
            builder.Register<LazyAttachBinder<AimView, AimViewModel>>(Lifetime.Singleton)
                .As<IViewBinder<AimView>>();
            builder.Register<Func<AimViewModel>>(x => () => x.Resolve<AimViewModel>(), Lifetime.Singleton);
        }

        private void InitUI()
        {
            var uiBinder = Container.Resolve<IRootUIBinder>();
            uiBinder.ClearViews();
            
            var hudBinder = Container.Resolve<IViewBinder<HUDView>>();
            hudBinder.Open();
            
            var aimBinder = Container.Resolve<IViewBinder<AimView>>();
            aimBinder.Open();
        }

        protected override void OnDestroy()
        {
            Disposes.ClearDispose(ref _disposables);
            base.OnDestroy();
        }
    }
}