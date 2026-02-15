using System;
using DevKit.Locator;
using DevKit.Saves;
using DevKit.UI.MVVM;
using DevKit.Utils;
using Inui.UI.MVVM.Settings;
using ITCafe.Data.Settings;
using ITCafe.Gameplay.Shared;
using ITCafe.Gameplay.UI.MVVM;
using ITCafe.Infrastructure.Saves;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ITCafe
{
    public class RootScope : LifetimeScope
    {
        [SerializeField] private RootUIBinder _rootUIBinderPrefab;
        [SerializeField] private SettingsView _settingsViewPrefab;
        [SerializeField] private SerializableLocalizationLoader _rootLocalizationLoader;
        [SerializeField] private SerializableLocalizationLoader _settingsLocalizationLoader;

        private CompositeDisposable _disposables = new();
        private RootUIBinder _rootUIBinder;

        protected override void Configure(IContainerBuilder builder)
        {
            FLogger.Log("RootScope Configure");
            ShaderUnscaledTime.On();

            var localizer = new UnityLocalizer();
            ServiceLocator.Current.Register<ILocalizer>(localizer);

            _rootUIBinder = Instantiate(_rootUIBinderPrefab);
            DontDestroyOnLoad(_rootUIBinder);
            builder.RegisterInstance<RootUIBinder>(_rootUIBinder)
                .As<IRootUIBinder>();

            builder.Register<SceneLoader>(Lifetime.Singleton);

            builder.Register<ILocalizationLoader>(_ => _rootLocalizationLoader, Lifetime.Scoped);
            builder.Register<ILocalizationLoader>(_ => _settingsLocalizationLoader, Lifetime.Scoped)
                .Keyed(Constants.SETTINGS_DATA_LOCALE_LOADER);

            RegisterSaves(builder);
            RegisterUI(builder);

            builder.RegisterBuildCallback(OnBuild);
        }

        private void RegisterSaves(IContainerBuilder builder)
        {
            var serializer = new JsonUtilitySerializer();
            var storage = new FileStorage(fileExtension: "json");
            var saveSystem = new SimpleSaveSystem(serializer, storage);
            var saveStateProvider = new SaveStateProvider(saveSystem);
            saveStateProvider.LoadAll();

            builder.RegisterInstance<ISaveStateProvider>(saveStateProvider);
            builder.RegisterInstance<SettingsState>(saveStateProvider.SaveState.SettingsState);
            builder.Register<SettingsModel>(Lifetime.Singleton);
        }

        private void RegisterUI(IContainerBuilder builder)
        {
            builder.RegisterInstance<SettingsView>(_settingsViewPrefab);
            builder.Register<SettingsViewModel>(Lifetime.Transient);
            builder.Register<Func<SettingsViewModel>>(x => () => x.Resolve<SettingsViewModel>(),
                Lifetime.Singleton);
            builder.Register<SettingsBinder>(Lifetime.Singleton)
                .As<IViewBinder<SettingsViewModel>>();
        }

        private void OnBuild(IObjectResolver container)
        {
            var loadingScreen = container.Resolve<LoadingScreen>();
            loadingScreen.transform.SetParent(_rootUIBinder.transform);
            InitSettings();
        }

        private void InitSettings()
        {
            var appSettingsModel = Container.Resolve<SettingsModel>();

            var appSettingsApplier = new AppSettingsApplier();
            appSettingsApplier.BindSettings(appSettingsModel)
                .AddTo(_disposables);
        }

        protected override void OnDestroy()
        {
            Disposes.ClearDispose(ref _disposables);
            base.OnDestroy();
        }
    }
}