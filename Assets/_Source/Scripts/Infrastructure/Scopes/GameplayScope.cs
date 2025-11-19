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
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;
using Cursor = UnityEngine.Cursor;
using Unit = R3.Unit;

namespace ITCafe
{
    public class GameplayScope : LifetimeScope
    {
        [Header("Player")]
        [SerializeField] private InputActionAsset _inputActionAsset;
        [SerializeField] private Interactor _playerInteractor;
        [SerializeField] private ItemPicker _playerItemPicker;
        [SerializeField] private InputActionReference _pauseActionRef;

        [Header("Clients"), Space(4)]
        [SerializeField] private ClientCharacter[] _clientPrefabs;
        [SerializeField] private AllItemInfoSO _allItemsInfoSO;
        [SerializeField] private Transform[] _clientSeatPoints;
        [SerializeField] private Transform[] _clientOrderPoints;

        [Header("UI")]
        [SerializeField] private AimView _aimViewPrefab;
        [SerializeField] private HUDView _hudViewPrefab;
        [SerializeField] private ResultsView _resultsViewPrefab;
        [SerializeField] private PauseView _pauseViewPrefab;

        private CompositeDisposable _disposables = new();
        private CancellationToken _destroyToken;
        private IViewBinder<PauseView> _pauseBinder;

        protected override void Configure(IContainerBuilder builder)
        {
            Time.timeScale = 1;
            RegisterUI(builder);

            builder.RegisterInstance<InputActionMap>(_inputActionAsset.FindActionMap("Player"));

            builder.Register<Subject<Unit>>(Lifetime.Singleton)
                .Keyed(Constants.GAMEPLAY_EXIT_SIGNAL);

            builder.Register<Subject<Unit>>(Lifetime.Singleton)
                .Keyed(Constants.RESTART_GAMEPLAY_SIGNAL);

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

            var inputService = Container.Resolve<InputService>();
            inputService.SetInputEnabled(false);

            InitUI();

            var loadingScreen = Container.Resolve<LoadingScreen>();
            loadingScreen.OnFinished.Take(1).Subscribe(_ => DelayedBoot());

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            var exitSignal = Container.Resolve<Subject<Unit>>(Constants.GAMEPLAY_EXIT_SIGNAL);
            var restartSignal = Container.Resolve<Subject<Unit>>(Constants.RESTART_GAMEPLAY_SIGNAL);

            var mainMenuEnterContext = new MainMenuEnterContext();
            
            var gameplayRestartContext = new GameplayExitContext(gameplayEnterContext ?? new GameplayEnterContext());
            var gameplayExitContext = new GameplayExitContext(mainMenuEnterContext);
            var gameplayExitSignal = new Subject<GameplayExitContext>();

            exitSignal.Take(1).Subscribe(_ => gameplayExitSignal.OnNext(gameplayExitContext))
                .AddTo(_disposables);

            restartSignal.Take(1).Subscribe(_ => gameplayExitSignal.OnNext(gameplayRestartContext))
                .AddTo(_disposables);


            return gameplayExitSignal;
        }

        private void DelayedBoot()
        {
            var inputService = Container.Resolve<InputService>();
            inputService.SetInputEnabled(true);

            _destroyToken = destroyCancellationToken;

            Container.Inject(_playerInteractor);
            _playerInteractor.Init();

            _playerInteractor.CanInteract.Subscribe(x => _playerItemPicker.IsDroppingBlocked = x)
                .AddTo(_disposables);
            _playerItemPicker.IsHoldingItem.Subscribe(x => Debug.Log($"Holding item: {x}"))
                .AddTo(_disposables);

            var sessionRunner = Container.Resolve<GameSessionRunner>();
            sessionRunner.RunSessionAsync(_destroyToken).Forget();
            sessionRunner.OnCompleted.Take(1).Subscribe(_ => UnsubscribePause())
                .AddTo(_disposables);

#if UNITY_EDITOR
            Observable.EveryUpdate().Where(_ => Keyboard.current.digit0Key.wasPressedThisFrame)
                .Take(1)
                .Subscribe(_ => sessionRunner.CompleteSession());
#endif

            SubscribePause();
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
                progressService.OnClientFailed.Subscribe(_ => hudViewModel.IncrementOrdersFailed());
                return hudViewModel;
            }, Lifetime.Singleton);

            builder.RegisterInstance<AimView>(_aimViewPrefab); // prefab registration
            builder.Register<AimViewModel>(Lifetime.Singleton);
            builder.Register<LazyAttachBinder<AimView, AimViewModel>>(Lifetime.Singleton)
                .As<IViewBinder<AimView>>();
            builder.Register<Func<AimViewModel>>(x => () => x.Resolve<AimViewModel>(), Lifetime.Singleton);

            builder.RegisterInstance<ResultsView>(_resultsViewPrefab); // prefab registration
            builder.Register<ResultsViewModel>(Lifetime.Singleton);
            builder.Register<LazyAttachBinder<ResultsView, ResultsViewModel>>(Lifetime.Singleton)
                .As<IViewBinder<ResultsView>>();
            builder.Register<Func<ResultsViewModel>>(x => () => x.Resolve<ResultsViewModel>(), Lifetime.Singleton);

            builder.RegisterInstance<PauseView>(_pauseViewPrefab); // prefab registration
            builder.Register<PauseViewModel>(Lifetime.Singleton);
            builder.Register<LazyAttachBinder<PauseView, PauseViewModel>>(Lifetime.Singleton)
                .As<IViewBinder<PauseView>>();
            builder.Register<Func<PauseViewModel>>(x => () => x.Resolve<PauseViewModel>(), Lifetime.Singleton);
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

        private void SubscribePause()
        {
            _pauseBinder = Container.Resolve<IViewBinder<PauseView>>();
            _pauseActionRef.action.started += Pause;
        }

        private void UnsubscribePause()
        {
            _pauseActionRef.action.started -= Pause;
        }

        private void Pause(InputAction.CallbackContext _)
        {
            _pauseBinder.Open();
        }

        protected override void OnDestroy()
        {
            UnsubscribePause();
            Disposes.ClearDispose(ref _disposables);
            base.OnDestroy();
        }
    }
}