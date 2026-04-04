using System;
using System.Collections;
using DevKit.Solutions;
using DevKit.UI.MVVM;
using DevKit.UI.MVVM.Bases;
using ITCafe.Campaign;
using ITCafe.Data.Campaign;
using ITCafe.Gameplay.Shared;
using ITCafe.Gameplay.UI.MVVM;
using ITCafe.Infrastructure.Saves;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ITCafe
{
    public class MainMenuScope : LifetimeScope
    {
        public Observable<MainMenuExitContext> ExitSignal { get; private set; }

        [SerializeField] private MainMenuView _mainMenuViewPrefab;
        [SerializeField] private CreditsView _creditsViewPrefab;
        [SerializeField] private CampaignView _campaignViewPrefab;
        [SerializeField] private SerializableLocalizationLoader _mainMenuLocalizationLoader;
        [SerializeField] private SerializableLocalizationLoader _campaignLocalizationLoader;
        [SerializeField] private AudioClip _mainMenuMusic;

        private MainMenuEnterContext _mainMenuEnterContext;
        private CompositeDisposable _disposables = new();

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterUI(builder);

            builder.Register<Subject<Unit>>(Lifetime.Scoped)
                .Keyed(Constants.START_MISSION_SIGNAL);

            builder.Register<LinearCampaignUnlocker>(Lifetime.Singleton);
            builder.Register<CampaignModelFactory>(Lifetime.Singleton)
                .AsSelf()
                .As<IFactory<CampaignModel>>();
            builder.Register<CampaignDataLoader>(Lifetime.Transient); // Transient
            builder.Register<CampaignDataModelFactory>(Lifetime.Singleton)
                .AsSelf()
                .As<IFactory<CampaignDataModel>>();

            builder.Register<ILocalizationLoader>(_ => _campaignLocalizationLoader, Lifetime.Scoped)
                .Keyed(Constants.CAMPAIGN_DATA_LOCALE_LOADER);
        }

        private void RegisterUI(IContainerBuilder builder)
        {
            builder.RegisterMVVM<CreditsView, CreditsViewModel>(_creditsViewPrefab);
            builder.RegisterMVVM<MainMenuView, MainMenuViewModel>(_mainMenuViewPrefab);
            builder.RegisterMVVM<CampaignView, CampaignViewModel, CampaignBinder>(_campaignViewPrefab, 
                Lifetime.Transient);
        }

        public IEnumerator BootCoroutine(MainMenuEnterContext mainMenuEnterContext = null)
        {
            Time.timeScale = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _mainMenuEnterContext = mainMenuEnterContext;

            Build();
            yield return new WaitForEndOfFrame();

            var audioPlayer = Container.Resolve<AudioPlayer>();
            audioPlayer.PlaySingletonMusic(_mainMenuMusic, loop: true);

            _mainMenuLocalizationLoader.Init();
            _mainMenuLocalizationLoader.AddTo(_disposables);
            yield return _mainMenuLocalizationLoader.LoadTables();

            var rootUIBinder = Container.Resolve<IRootUIBinder>();
            rootUIBinder.ClearViews();

            var mainMenuBinder = Container.Resolve<IViewBinder<MainMenuViewModel>>();
            mainMenuBinder.Open();

            var exitSignal = Container.Resolve<Subject<Unit>>(Constants.START_MISSION_SIGNAL);
            var gameplayEnterContext = new GameplayEnterContext()
            {
                ToSceneName = Scenes.GAMEPLAY_1,
                LocationId = "location_1",
                MissionId = "mission_1_1"
            };

            var mainMenuExitContext = new MainMenuExitContext(gameplayEnterContext);
            var mainMenuExitSignal = new Subject<MainMenuExitContext>();

            exitSignal.Take(1).Subscribe(_ =>
            {
                // Fetching data from all models for enter context
                // use another class for main menu local context mb
                var campaignDataModelFactory = Container.Resolve<CampaignDataModelFactory>();
                var campaignDataModel = campaignDataModelFactory.CurrentInstance.CurrentValue; // Took last model

                var selectedLocationId = campaignDataModel.SelectedLocationData.Value.Id;
                var selectedMissionId = campaignDataModel.SelectedMissionData.Value.Id;

                gameplayEnterContext.ToSceneName = campaignDataModel.SelectedMissionData.Value.SceneName;
                gameplayEnterContext.LocationId = selectedLocationId;
                gameplayEnterContext.MissionId = selectedMissionId;

                var campaignModelFactory = Container.Resolve<CampaignModelFactory>();
                var campaignModel = campaignModelFactory.Current.CurrentValue;
                var campaignUnlocker = Container.Resolve<LinearCampaignUnlocker>();
                var saveStateProvider = Container.Resolve<ISaveStateProvider>();
                var selectedMissionModel = campaignModel.OpenedLocationsMap[selectedLocationId]
                    .OpenedMissionsMap[selectedMissionId];
                var completionSignal = campaignUnlocker.CreateMissionCompletionSignal<CafeMissionResult>(campaignModel,
                    campaignDataModel, saveStateProvider.SaveAll, (result, _) =>
                    {
                        var starsCount = result.Stars;
                        if (selectedMissionModel.Stars.Value < starsCount)
                            selectedMissionModel.Stars.Value = starsCount;

                        return starsCount > 0;
                    });
                gameplayEnterContext.CompletionSignal = completionSignal;

                mainMenuExitSignal.OnNext(mainMenuExitContext);
            });

            ExitSignal = mainMenuExitSignal;
        }

        protected override void OnDestroy()
        {
            Disposes.ClearDispose(ref _disposables);
            base.OnDestroy();
        }
    }
}