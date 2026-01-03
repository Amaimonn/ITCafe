using System;
using System.Collections;
using DevKit.Solutions;
using DevKit.UI.MVVM;
using DevKit.UI.MVVM.Bases;
using ITCafe.Campaign;
using ITCafe.Data.Campaign;
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
        [SerializeField] private CampaignView _campaignViewPrefab;

        private MainMenuEnterContext _mainMenuEnterContext;

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
        }

        private void RegisterUI(IContainerBuilder builder)
        {
            builder.RegisterInstance<MainMenuView>(_mainMenuViewPrefab);
            builder.Register<MainMenuViewModel>(Lifetime.Singleton);
            builder.Register<Func<MainMenuViewModel>>(x => () => x.Resolve<MainMenuViewModel>(), Lifetime.Singleton);
            builder.Register<SimpleAttachBinder<MainMenuView, MainMenuViewModel>>(Lifetime.Singleton)
                .As<IViewBinder<MainMenuViewModel>>();

            builder.RegisterInstance<CampaignView>(_campaignViewPrefab);
            builder.Register<CampaignViewModel>(Lifetime.Transient); // Transient
            builder.Register<Func<CampaignViewModel>>(x => () => x.Resolve<CampaignViewModel>(), Lifetime.Singleton);
            builder.Register<CampaignBinder>(Lifetime.Singleton)
                .As<IViewBinder<CampaignViewModel>>();
        }

        public IEnumerator BootCoroutine(MainMenuEnterContext mainMenuEnterContext = null)
        {
            Time.timeScale = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _mainMenuEnterContext = mainMenuEnterContext;

            Build();
            yield return new WaitForEndOfFrame();

            var rootUIBinder = Container.Resolve<IRootUIBinder>();
            rootUIBinder.ClearViews();

            var mainMenuBinder = Container.Resolve<IViewBinder<MainMenuViewModel>>();
            mainMenuBinder.Open();

            var exitSignal = Container.Resolve<Subject<Unit>>(Constants.START_MISSION_SIGNAL);
            var gameplayEnterContext = new GameplayEnterContext()
            {
                // TODO: define through UI
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
                        if (selectedMissionModel.Stars.Value < result.Stars)
                            selectedMissionModel.Stars.Value = result.Stars;
                    });
                gameplayEnterContext.CompletionSignal = completionSignal;
                
                mainMenuExitSignal.OnNext(mainMenuExitContext);
            });

            ExitSignal = mainMenuExitSignal;
        }
    }
}