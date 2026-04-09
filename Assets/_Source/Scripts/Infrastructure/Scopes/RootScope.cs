using DevKit.Locator;
using DevKit.Saves;
using DevKit.UI.MVVM;
using DevKit.Utils;
using Inui.UI.MVVM.Settings;
using ITCafe.Data.Settings;
using ITCafe.Shared;
using ITCafe.UI.MVVM;
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
        [SerializeField] private ConfirmPopUpView _confirmPopUpPrefab;
        [SerializeField] private SerializableLocalizationLoader _rootLocalizationLoader;
        [SerializeField] private SerializableLocalizationLoader _settingsLocalizationLoader;
        [SerializeField] private AudioPlayer _audioPlayer;
            
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
            RegisterAudio(builder);

            builder.RegisterBuildCallback(OnBuild);
        }

        private void RegisterSaves(IContainerBuilder builder)
        {
            var serializer = new NewtonsoftSerializer();
            var storage = new FileStorage(fileExtension: "json");
            var saveSystem = new SimpleSaveSystem(serializer, storage);
            var saveStateProvider = new SaveStateProvider(saveSystem);
            saveStateProvider.LoadAll();

            builder.RegisterInstance<ISaveStateProvider>(saveStateProvider);
            
            var saveState = saveStateProvider.SaveState;
            var settingsState = saveState.SettingsState;
            builder.RegisterInstance<SettingsState>(settingsState);
            builder.Register<SettingsModel>(Lifetime.Singleton);
        }

        private void RegisterUI(IContainerBuilder builder)
        {
            builder.RegisterMVVM<SettingsView, SettingsViewModel, SettingsBinder>(_settingsViewPrefab, 
                viewModelLifetime: Lifetime.Transient); 
            
            // PopUp registration with transient binder
            builder.RegisterMVVM<ConfirmPopUpView, ConfirmPopUpViewModel>(_confirmPopUpPrefab, 
                viewModelLifetime: Lifetime.Transient, 
                binderLifetime: Lifetime.Transient);
        }

        private void RegisterAudio(IContainerBuilder builder)
        {
            builder.Register<AudioPlayer>(resolver => 
            {
                var settingsModel = resolver.Resolve<SettingsModel>();
                
                _audioPlayer.Init(musicVolume: settingsModel.MusicVolume.Select(x => x / 100.0f), 
                    sfxVolume: settingsModel.SfxVolume.Select(x => x / 100.0f));
                
                ServiceLocator.Current.Register<AudioPlayer>(_audioPlayer);
                
                var loadingScreen = resolver.Resolve<LoadingScreen>();
                loadingScreen.OverlayFillProgress.Subscribe(x => _audioPlayer.VolumeMultiplier.Value = 1.0f - x);
                
                var sceneLoader = resolver.Resolve<SceneLoader>();
                sceneLoader.OnLoadingStarted.Subscribe(_ => 
                {
                    _audioPlayer.StopAllSfx();
                    _audioPlayer.PauseMusic();
                });
                sceneLoader.OnLoadingFinished.Subscribe(_ =>
                {
                    _audioPlayer.UnPauseMusic();
                });
                
                return _audioPlayer;
            }, Lifetime.Singleton);
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