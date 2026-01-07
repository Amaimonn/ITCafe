using System.Linq;
using DevKit.Utils;
using DevKit.UI.MVVM.Bases;
using ITCafe.Data.Settings;
using ITCafe.Gameplay.UI.MVVM;
using ITCafe.Infrastructure.Saves;
using R3;

namespace Inui.UI.MVVM.Settings
{
    public class SettingsViewModel : ScreenViewModel
    {
        public Observable<ISettingsData> OnSettingsDataChanged => _settingsData;
        public Observable<bool> IsAnyChanges => _isAnyChanges;
        public Observable<SettingsSectionViewModel> OnSectionChanged => _currentSection;

        public readonly VideoSettingsViewModel VideoSettingsViewModel = new();
        public readonly SoundSettingsViewModel SoundSettingsViewModel = new();
        public readonly LanguageSectionViewModel LanguageSettingsViewModel = new();

        private readonly ISaveStateProvider _gameStateProvider;

        private ReadOnlyReactiveProperty<bool> _isAnyChanges;
        private SettingsModel _model;
        private readonly ReactiveProperty<ISettingsData> _settingsData = new();
        private ReactiveProperty<SettingsSectionViewModel> _currentSection;

        public SettingsViewModel(ISaveStateProvider gameStateProvider)
        {
            _gameStateProvider = gameStateProvider;
        }

        public void Bind(SettingsModel model)
        {
            _model = model;
            VideoSettingsViewModel.Bind(model);
            SoundSettingsViewModel.Bind(model);
            LanguageSettingsViewModel.Bind(model);

            _isAnyChanges = Observable.CombineLatest(VideoSettingsViewModel.IsAnyChanges,
                    SoundSettingsViewModel.IsAnyChanges, 
                    LanguageSettingsViewModel.IsAnyChanges)
                .Select(x => x.Any(t => t))
                .ToReadOnlyReactiveProperty();
            
            _currentSection = new ReactiveProperty<SettingsSectionViewModel>(VideoSettingsViewModel);
        }

        public void SetSettingsData(ISettingsData settingsData)
        {
            _settingsData.Value = settingsData;
        }

        public override void StartClosing()
        {
            CancelUnappliedChanges();
            base.StartClosing();
        }

        /// <summary>
        /// Применяет все изменения
        /// </summary>
        public void ApplyChanges()
        {
            VideoSettingsViewModel.ApplyChanges();
            _model.ApplyToState();
            SaveSettings();
            FLogger.Log("Settings: Applied");
        }

        public void CancelUnappliedChanges()
        {
            VideoSettingsViewModel.CancelChanges();
            FLogger.Log("Settings: Cancelled");
        }

        public void SelectSection(SettingsSectionViewModel section)
        {
            _currentSection.Value = section;
        }

        private void SaveSettings()
        {
            _gameStateProvider.SaveAll();
        }

        public override void Dispose()
        {
            VideoSettingsViewModel.Dispose();
            base.Dispose();
        }
    }
}