using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DevKit.Solutions;
using DevKit.UI.MVVM;
using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using ITCafe.Gameplay.CafeBusiness;
using ITCafe.Data;
using ITCafe.Data.Campaign;
using ITCafe.Gameplay.UI.MVVM;
using ITCafe.Player;
using R3;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;
using Cursor = UnityEngine.Cursor;
using Unit = R3.Unit;
using ITCafe.Data.Settings;

namespace ITCafe
{
    public class GameplayScope : LifetimeScope
    {
        public Observable<GameplayExitContext> ExitSignal { get; private set; }

        [Header("Player")]
        [SerializeField] private InputActionAsset _inputActionAsset;
        [SerializeField] private Interactor _playerInteractor;
        [SerializeField] private ItemPicker _playerItemPicker;
        [SerializeField] private string _pauseActionName;
        [SerializeField] private CinemachineInputAxisController _cinemachineInputAxisController;

        [Header("Clients"), Space(4)]
        [SerializeField] private ClientCharacter[] _clientPrefabs;
        [SerializeField] private Transform[] _clientSeatPoints;
        [SerializeField] private Transform[] _clientOrderPoints;

        [Header("UI"), Space(4)]
        [SerializeField] private AimView _aimViewPrefab;
        [SerializeField] private HUDView _hudViewPrefab;
        [SerializeField] private ResultsView _resultsViewPrefab;
        [SerializeField] private PauseView _pauseViewPrefab;
        [SerializeField] private GuideView _guideViewPrefab;

        [Header("Other"), Space(4)]
        [SerializeField] private GameObject _missionSetupRoot;
        [SerializeField] private LocalizationLoader _localizationLoader;

        private GuideSO _guideSO;
        private CompositeDisposable _disposables = new();
        private CancellationToken _destroyToken;
        private IViewBinder<PauseViewModel> _pauseBinder;
        private GameplayEnterContext _gameplayEnterContext;
        private InputAction _pauseActionRef;
        private MissionSetupSO _missionSetup;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance<Subject<CafeMissionResult>>(_gameplayEnterContext.CompletionSignal);

            RegisterMissionConfig(builder);

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

            builder.Register<CraftService>(Lifetime.Singleton)
                .As<ICraftService>();
        }

        private void RegisterMissionConfig(IContainerBuilder builder)
        {
            builder.RegisterInstance<MissionSetupSO>(_missionSetup);

            var menuItemsMap = new Dictionary<ItemTag, ItemInfoSO>();
            var menuItemsHashMap = new Dictionary<int, ItemInfoSO>();
            var allItemsMap = new Dictionary<ItemTag, ItemInfoSO>();

            foreach (var itemInfo in _missionSetup.ItemsInfoSO.AllInfo)
            {
                if (!allItemsMap.TryAdd(itemInfo.ItemTag, itemInfo))
                    FLogger.LogWarning($"{itemInfo.ItemTag} has already been added");

                if (itemInfo.MenuItemExtra != null)
                {
                    menuItemsMap.TryAdd(itemInfo.ItemTag, itemInfo);
                    menuItemsHashMap.TryAdd(itemInfo.MenuItemExtra.ItemInfo.GetItemHash(), itemInfo);
                }
            }

            builder.RegisterInstance<Dictionary<ItemTag, ItemInfoSO>>(allItemsMap)
                .AsSelf()
                .As<IReadOnlyDictionary<ItemTag, ItemInfoSO>>()
                .Keyed(Constants.ALL_ITEMS_MAP);

            builder.RegisterInstance<Dictionary<ItemTag, ItemInfoSO>>(menuItemsMap)
                .AsSelf()
                .As<IReadOnlyDictionary<ItemTag, ItemInfoSO>>()
                .Keyed(Constants.MENU_ITEMS_MAP);

            builder.RegisterInstance<Dictionary<int, ItemInfoSO>>(menuItemsHashMap)
                .AsSelf()
                .As<IReadOnlyDictionary<int, ItemInfoSO>>()
                .Keyed(Constants.MENU_ITEMS_HASH_MAP);

            builder.RegisterInstance<IEnumerable<RecipeSO>>(_missionSetup.RecipesSO.Recipes);

            builder.RegisterInstance<MissionEvaluation>(_missionSetup.MissionEvaluation);

            _guideSO = _missionSetup.GuideSO;
            if (_guideSO != null)
                builder.RegisterInstance<GuideSO>(_guideSO);

            var itemsInfo = _missionSetup.ItemsInfoSO.AllInfo;
            builder.RegisterInstance<ItemInfoSO[]>(itemsInfo)
                .AsSelf()
                .As<IEnumerable<ItemInfoSO>>();
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

        public IEnumerator BootCoroutine(GameplayEnterContext gameplayEnterContext = null)
        {
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _inputActionAsset.FindActionMap("GameUI").Enable();

            _gameplayEnterContext = gameplayEnterContext;

            // TODO: take id from enterContext
            var setupId = gameplayEnterContext == null ? "mission_1_1_setup" : $"{gameplayEnterContext.MissionId}_setup";

            var handle = Addressables.LoadAssetAsync<MissionSetupSO>(setupId);
            yield return handle;

            _missionSetup = handle.Result;

            Build();
            yield return new WaitForEndOfFrame();
            
            _localizationLoader.Init();
            _localizationLoader.AddTo(_disposables);
            yield return _localizationLoader.LoadTables();

            // Scene setup
            Destroy(_missionSetupRoot);
            var sceneSetup = _missionSetup.SceneObjectsPrefab;
            yield return InstantiateAsync(sceneSetup);

            // Input init
            var inputService = Container.Resolve<InputService>();
            inputService.SetInputEnabled(false);

            InitUI();
            yield return new WaitForEndOfFrame();

            // Delayed boot setup
            var loadingScreen = Container.Resolve<LoadingScreen>();
            loadingScreen.OnFinished.Take(1).Subscribe(_ => BootAfterLoading());

            // Item prefabs registering
            var itemsCreator = Container.Resolve<ItemsCreator>();
            var itemPrefabs = Container.Resolve<IReadOnlyDictionary<ItemTag, ItemInfoSO>>(Constants.ALL_ITEMS_MAP);
            foreach (var entry in itemPrefabs.Values)
            {
                if (entry.Prefab != null)
                    itemsCreator.Register(entry.Prefab, entry.ItemTag);
            }

            // Settings binding
            var settingsModel = Container.Resolve<SettingsModel>();
            settingsModel.Sensitivity.Subscribe(x =>
                {
                    var newValue = x <= 50f
                        ? Mathf.Lerp(0.2f, 1f, Mathf.InverseLerp(1f, 50f, x))
                        : Mathf.Lerp(1f, 5f, Mathf.InverseLerp(50f, 100f, x));

                    foreach (var c in _cinemachineInputAxisController.Controllers)
                    {
                        c.Input.Gain = c.Name switch
                        {
                            "Look X (Pan)" => newValue,
                            "Look Y (Tilt)" => -newValue,
                            _ => c.Input.Gain
                        };
                    }
                })
                .AddTo(_disposables);

            // Exit callback setup
            var exitSignal = Container.Resolve<Subject<Unit>>(Constants.GAMEPLAY_EXIT_SIGNAL);
            var restartSignal = Container.Resolve<Subject<Unit>>(Constants.RESTART_GAMEPLAY_SIGNAL);

            var mainMenuEnterContext = new MainMenuEnterContext();

            var gameplayRestartContext = new GameplayExitContext(gameplayEnterContext ??
                                                                 new GameplayEnterContext()
                                                                 {
                                                                     ToSceneName = Scenes.GAMEPLAY_1,
                                                                     MissionId = "mission_1_1",
                                                                     LocationId = "Location_1_1",
                                                                 });
            var gameplayExitContext = new GameplayExitContext(mainMenuEnterContext);
            var gameplayExitSignal = new Subject<GameplayExitContext>();

            exitSignal.Take(1)
                .Subscribe(_ => gameplayExitSignal.OnNext(gameplayExitContext))
                .AddTo(_disposables);

            restartSignal.Take(1)
                .Subscribe(_ => gameplayExitSignal.OnNext(gameplayRestartContext))
                .AddTo(_disposables);

            ExitSignal = gameplayExitSignal;
        }

        private void BootAfterLoading()
        {
            if (_guideSO != null)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                var guideBinder = Container.Resolve<IViewBinder<GuideViewModel>>();
                var guideViewModel = guideBinder.Open();
                guideViewModel.OnClosingCompleted
                    .Take(1)
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
            sessionRunner.RunSessionAsync(token: _destroyToken).Forget();
            sessionRunner.OnCompleted.Take(1).Subscribe(_ => UnsubscribePause())
                .AddTo(_disposables);

#if UNITY_EDITOR
            Observable.EveryUpdate().Where(_ => Keyboard.current.digit0Key.wasPressedThisFrame)
                .Take(1)
                .Subscribe(_ => sessionRunner.CompleteSession());
#endif

            SubscribePause();
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
            _pauseActionRef = _inputActionAsset.FindAction(_pauseActionName);

            if (_pauseActionRef != null)
            {
                _pauseActionRef.started += Pause;
                FLogger.LogGood<GameplayScope>("Pause action binded");
            }
            else
            {
                FLogger.LogError<GameplayScope>("Pause action not found");
            }
        }

        private void UnsubscribePause()
        {
            if (_pauseActionRef != null)
                _pauseActionRef.started -= Pause;
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