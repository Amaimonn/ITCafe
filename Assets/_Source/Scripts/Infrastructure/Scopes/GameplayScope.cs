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
using ITCafe.Gameplay.Data;
using ITCafe.Gameplay.UI.MVVM;
using ITCafe.Player;
using R3;
using Unity.Cinemachine;
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
        [Serializable]
        public struct KeyedGameObject
        {
            public ItemTag KeyTag;
            public GameObject GameObject;
        }

        [Header("Player")]
        [SerializeField] private InputActionAsset _inputActionAsset;
        [SerializeField] private Interactor _playerInteractor;
        [SerializeField] private ItemPicker _playerItemPicker;
        [SerializeField] private InputActionReference _pauseActionRef;
        [SerializeField] private CinemachineInputAxisController _cinemachineInputAxisController;

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
        [SerializeField] private GuideView _guideViewPrefab;

        [Header("Other")]
        [SerializeField] private KeyedGameObject[] _keyedItemPrefabs;
        [SerializeField] private AllRecipesSO _recipesSO;
        [SerializeField] private GuideSO _guideSO;

        private CompositeDisposable _disposables = new();
        private CancellationToken _destroyToken;
        private IViewBinder<PauseViewModel> _pauseBinder;

        protected override void Configure(IContainerBuilder builder)
        {
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

            builder.Register<IReadOnlyDictionary<int, ItemInfoSO>>(x =>
                _allItemsInfoSO.AllInfo.ToDictionary(y => y.ItemInfo.GetItemHash()), Lifetime.Singleton);

            builder.RegisterComponent<IItemPicker>(_playerItemPicker);

            builder.Register<PlayerContext>(Lifetime.Singleton);

            builder.Register<InputService>(Lifetime.Singleton);

            builder.Register<ClientsFactory>(Lifetime.Singleton)
                .As<IFactory<ClientCharacter>>();

            builder.Register<ClientsRunner>(Lifetime.Singleton);

            builder.Register<GameSessionRunner>(Lifetime.Singleton);

            builder.Register<WorkProgressService>(Lifetime.Singleton);

            builder.Register<ItemsCreator>(Lifetime.Singleton)
                .AsSelf()
                .As<IItemsCreator>();

            builder.RegisterInstance<IEnumerable<RecipeSO>>(_recipesSO.Recipes);

            builder.Register<CraftService>(Lifetime.Singleton)
                .As<ICraftService>();

            if (_guideSO != null)
                builder.RegisterInstance<GuideSO>(_guideSO);
        }

        public Observable<GameplayExitContext> Boot(GameplayEnterContext gameplayEnterContext = null)
        {
            Time.timeScale = 1;

            Build();

            var inputService = Container.Resolve<InputService>();
            inputService.SetInputEnabled(false);

            InitUI();

            var loadingScreen = Container.Resolve<LoadingScreen>();
            loadingScreen.OnFinished.Take(1).Subscribe(_ => BootAfterLoading());

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            var itemsCreator = Container.Resolve<ItemsCreator>();
            foreach (var entry in _keyedItemPrefabs)
                itemsCreator.Register(entry.GameObject, entry.KeyTag);

            var settingsModel = Container.Resolve<SettingsModel>();
            settingsModel.Sensitivity.Subscribe(x =>
                {
                    var newValue = x <= 50f
                        ? Mathf.Lerp(0.2f, 1f, Mathf.InverseLerp(1f, 50f, x))
                        : Mathf.Lerp(1f, 5f, Mathf.InverseLerp(50f, 100f, x));
                    foreach (var c in _cinemachineInputAxisController.Controllers)
                    {
                        if (c.Name == "Look X (Pan)")
                        {
                            c.Input.Gain = newValue;
                        }
                        if (c.Name == "Look Y (Tilt)")
                        {
                            c.Input.Gain = -newValue;
                        }
                    }
                })
                .AddTo(_disposables);

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

        private void BootAfterLoading()
        {
            if (_guideSO != null)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                var guideBinder = Container.Resolve<IViewBinder<GuideViewModel>>();
                var guideViewModel = guideBinder.Open();
                guideViewModel.OnClosingCompleted.Take(1)
                    .Subscribe(_ => BootAfterGuide())
                    .AddTo(_disposables);
            }
            else
            {
                BootAfterGuide();
            }
        }

        private void BootAfterGuide()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            var inputService = Container.Resolve<InputService>();
            inputService.SetInputEnabled(true);

            _destroyToken = destroyCancellationToken;

            Container.Inject(_playerInteractor);
            _playerInteractor.Init();

            var playerContext = Container.Resolve<PlayerContext>();
            _playerItemPicker.Init(playerContext);

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
            builder.Register<SimpleAttachBinder<HUDView, HUDViewModel>>(Lifetime.Singleton)
                .As<IViewBinder<HUDViewModel>>();
            builder.Register<Func<HUDViewModel>>(x => () =>
            {
                var hudViewModel = x.Resolve<HUDViewModel>();
                var progressService = x.Resolve<WorkProgressService>();
                progressService.OnOrderTaken.Subscribe(_ => hudViewModel.IncrementOrdersTaken());
                progressService.OnClientServed.Subscribe(_ => hudViewModel.IncrementOrdersCompleted());
                progressService.OnClientFailed.Subscribe(_ => hudViewModel.IncrementOrdersFailed());
                return hudViewModel;
            }, Lifetime.Singleton);

            builder.RegisterInstance<AimView>(_aimViewPrefab); // prefab registration
            builder.Register<AimViewModel>(Lifetime.Singleton);
            builder.Register<SimpleAttachBinder<AimView, AimViewModel>>(Lifetime.Singleton)
                .As<IViewBinder<AimViewModel>>();
            builder.Register<Func<AimViewModel>>(x => () => x.Resolve<AimViewModel>(), Lifetime.Singleton);

            builder.RegisterInstance<ResultsView>(_resultsViewPrefab); // prefab registration
            builder.Register<ResultsViewModel>(Lifetime.Singleton);
            builder.Register<SimpleAttachBinder<ResultsView, ResultsViewModel>>(Lifetime.Singleton)
                .As<IViewBinder<ResultsViewModel>>();
            builder.Register<Func<ResultsViewModel>>(x => () => x.Resolve<ResultsViewModel>(), Lifetime.Singleton);

            builder.RegisterInstance<PauseView>(_pauseViewPrefab); // prefab registration
            builder.Register<PauseViewModel>(Lifetime.Singleton);
            builder.Register<SimpleAttachBinder<PauseView, PauseViewModel>>(Lifetime.Singleton)
                .As<IViewBinder<PauseViewModel>>();
            builder.Register<Func<PauseViewModel>>(x => () => x.Resolve<PauseViewModel>(), Lifetime.Singleton);

            if (_guideSO != null)
            {
                builder.RegisterInstance<GuideView>(_guideViewPrefab);
                builder.Register<GuideViewModel>(Lifetime.Singleton);
                builder.Register<SimpleAttachBinder<GuideView, GuideViewModel>>(Lifetime.Singleton)
                    .As<IViewBinder<GuideViewModel>>();
                builder.Register<Func<GuideViewModel>>(x => () => x.Resolve<GuideViewModel>(), Lifetime.Singleton);
            }
        }

        private void InitUI()
        {
            var uiBinder = Container.Resolve<IRootUIBinder>();
            uiBinder.ClearViews();

            var hudBinder = Container.Resolve<IViewBinder<HUDViewModel>>();
            hudBinder.Open();

            var aimBinder = Container.Resolve<IViewBinder<AimViewModel>>();
            aimBinder.Open();
        }

        private void SubscribePause()
        {
            _pauseBinder = Container.Resolve<IViewBinder<PauseViewModel>>();
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