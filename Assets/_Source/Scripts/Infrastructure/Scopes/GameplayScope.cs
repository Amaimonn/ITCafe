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
using ITCafe.Gameplay.Shared;
using UnityEngine.Rendering;

namespace ITCafe
{
    public class GameplayScope : LifetimeScope
    {
        public R3.Observable<GameplayExitContext> ExitSignal { get; private set; }

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
        [SerializeField] private SerializableLocalizationLoader _localizationLoader;
        [SerializeField] private Volume _volume;
        [SerializeField] private AudioClip _gameplayMusic;

        private IGuideData _guideData;
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

            builder.RegisterInstance<CinemachineInputAxisController>(_cinemachineInputAxisController);

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

            builder.Register<GameStatsService>(Lifetime.Singleton);

            builder.Register<ItemsCreator>(Lifetime.Singleton)
                .AsSelf()
                .As<IItemsCreator>();

            builder.Register<CraftService>(Lifetime.Singleton)
                .As<ICraftService>();

            builder.Register<PostProcessingSettingsApplier>(Lifetime.Singleton);
            builder.RegisterInstance<Volume>(_volume);
        }

        private void RegisterMissionConfig(IContainerBuilder builder)
        {
            builder.RegisterInstance<MissionSetupSO>(_missionSetup);

            var menuItemsMap = new Dictionary<ItemTag, ItemInfoSO>();
            var menuItemsHashMap = new Dictionary<int, ItemInfoSO>();
            var allItemsMap = new Dictionary<ItemTag, ItemInfoSO>();

            foreach (var itemInfo in _missionSetup.ItemInfoCollection.AllInfo)
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

            builder.RegisterInstance<IEnumerable<IRecipeData>>(_missionSetup.RecipeCollection.Recipes);

            builder.RegisterInstance<IMissionEvaluation>(_missionSetup.MissionEvaluation);


            if (_missionSetup.GuideData != null && _missionSetup.GuideData.Pages.Count > 0)
            {
                _guideData = _missionSetup.GuideData;
                builder.RegisterInstance<IGuideData>(_guideData);
            }

            var itemsInfo = _missionSetup.ItemInfoCollection.AllInfo;
            builder.RegisterInstance<IReadOnlyList<ItemInfoSO>>(itemsInfo)
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
                var progressService = x.Resolve<GameStatsService>();
                
                progressService.OnScoreChanged.Subscribe(hudViewModel.SetScore)
                    .AddTo(_disposables);
                progressService.OnOrderTaken.Subscribe(_ => hudViewModel.IncrementOrdersTaken())
                    .AddTo(_disposables);
                progressService.OnClientServed.Subscribe(_ => hudViewModel.IncrementOrdersCompleted())
                    .AddTo(_disposables);
                progressService.OnClientFailed.Subscribe(_ => hudViewModel.IncrementOrdersFailed())
                    .AddTo(_disposables);
                
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

            if (_guideData != null)
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

            _gameplayEnterContext = gameplayEnterContext ??
                                    new GameplayEnterContext
                                    {
                                        ToSceneName = Scenes.GAMEPLAY_1,
                                        MissionId = "mission_dev",
                                        LocationId = "location_1",
                                        CompletionSignal = new Subject<CafeMissionResult>()
                                    };

            var setupId = $"{_gameplayEnterContext.MissionId}_setup";
            var handle = Addressables.LoadAssetAsync<MissionSetupSO>(setupId);
            _disposables.Add(Disposable.Create(() => handle.Release()));

            yield return handle;

            _missionSetup = handle.Result;

            Build();

            yield return new WaitForEndOfFrame();
            
            var audioPlayer = Container.Resolve<AudioPlayer>();
            audioPlayer.PlaySingletonMusic(_gameplayMusic, loop: true);

            _localizationLoader.Init();
            _localizationLoader.AddTo(_disposables);
            yield return _localizationLoader.LoadTables();

            // Mission Setup
            Destroy(_missionSetupRoot);
            var sceneSetup = _missionSetup.SceneSetupPrefab;

            yield return InstantiateAsync(sceneSetup);

            var tableCollection = _missionSetup.LocaleTableCollection;
            if (tableCollection is { TableReferences: { Count: > 0 } })
            {
                var localizationLoadingService = new LocalizationLoadingService();
                localizationLoadingService.Init(tableCollection);
                localizationLoadingService.AddTo(_disposables);

                yield return localizationLoadingService.LoadTables();
            }

            // Input init
            var inputService = Container.Resolve<InputService>();
            inputService.SetInputEnabled(false);

            InitUI();

            yield return new WaitForEndOfFrame();

            InitSettings();

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

            // Exit callback setup
            var exitSignal = Container.Resolve<Subject<Unit>>(Constants.GAMEPLAY_EXIT_SIGNAL);
            var restartSignal = Container.Resolve<Subject<Unit>>(Constants.RESTART_GAMEPLAY_SIGNAL);

            var mainMenuEnterContext = new MainMenuEnterContext();

            var gameplayRestartContext = new GameplayExitContext(_gameplayEnterContext);
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
            if (_guideData != null)
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

        private void InitSettings()
        {
            var settingsModel = Container.Resolve<SettingsModel>();
            var postProcessingController = Container.Resolve<PostProcessingSettingsApplier>();

            postProcessingController.BindSettings(settingsModel)
                .AddTo(_disposables);
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