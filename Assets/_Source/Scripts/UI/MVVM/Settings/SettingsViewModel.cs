using System.Collections.Generic;
using System.Linq;
using DevKit.Utils;
using DevKit.UI.MVVM.Bases;
using ITCafe.Data.Settings;
using ITCafe.UI.MVVM;
using ITCafe.Infrastructure.Saves;
using ObservableCollections;
using R3;

namespace Inui.UI.MVVM.Settings
{
    public class SettingsViewModel : ScreenViewModel
    {
        public Observable<bool> IsAnyChanges => _isAnyChanges;
        public Observable<SettingsSectionViewModel> OnSectionChanged => _currentSection;
        public Observable<ISettingsData> OnSettingsDataChanged => _settingsData;

        public readonly VideoSettingsViewModel VideoSettingsViewModel = new();
        public readonly SoundSettingsViewModel SoundSettingsViewModel = new();
        public readonly LanguageSectionViewModel LanguageSettingsViewModel = new();
        public readonly InputSectionViewModel InputSettingsViewModel = new();

        private readonly ISaveStateProvider _gameStateProvider;

        private ReadOnlyReactiveProperty<bool> _isAnyChanges;
        private SettingsModel _model;
        private readonly ReactiveProperty<ISettingsData> _settingsData = new();
        private ReactiveProperty<SettingsSectionViewModel> _currentSection;
        private readonly List<SettingsSectionViewModel> _sections;

        public SettingsViewModel(ISaveStateProvider gameStateProvider)
        {
            _gameStateProvider = gameStateProvider;
            _sections = new List<SettingsSectionViewModel>
            {
                VideoSettingsViewModel,
                SoundSettingsViewModel,
                LanguageSettingsViewModel,
                InputSettingsViewModel
            };
        }

        public void Bind(SettingsModel model)
        {
            _model = model;
            foreach (var section in _sections)
                section.Bind(model);

            _isAnyChanges = Observable.CombineLatest(_sections.Select(x => x.IsAnyChanges))
                .Select(x => x.Any(t => t))
                .ToReadOnlyReactiveProperty();
            
            _currentSection = new ReactiveProperty<SettingsSectionViewModel>(VideoSettingsViewModel);
        }

        public void SetSettingsData(ISettingsData settingsData)
        {
            _settingsData.Value = settingsData;
            foreach (var section in _sections)
                section.SetSettingsData(settingsData);
        }

        public override void StartClosing()
        {
            CancelUnappliedChanges();
            base.StartClosing();
        }

        public void ApplyChanges()
        {
            foreach (var section in _sections)
                section.ApplyChanges();
            
            _model.ApplyToState();
            SaveSettings();
            
            FLogger.Log("Settings: Applied");
        }

        public void CancelUnappliedChanges()
        {
            foreach (var section in _sections)
                section.CancelChanges();
            
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