using System;
using System.Collections.Generic;
using System.Linq;
using DevKit.UI.MVVM;
using DevKit.Utils;
using DevKit.UI.MVVM.Bases;
using ITCafe.Data;
using ITCafe.Data.Settings;
using ITCafe.UI.MVVM;
using ITCafe.Infrastructure.Saves;
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

        private readonly IViewBinder<ConfirmPopUpViewModel> _confirmBinder;
        private readonly ISaveStateProvider _gameStateProvider;

        private ReadOnlyReactiveProperty<bool> _isAnyChanges;
        private SettingsModel _model;
        private readonly ReactiveProperty<ISettingsData> _settingsData = new();
        private ReactiveProperty<SettingsSectionViewModel> _currentSection;
        private readonly List<SettingsSectionViewModel> _sections;
        private ConfirmPopUpViewModel _confirmViewModel;

        private IDisposable _popUpConfirmDisposable;
        private ConfirmationSetup _confirmSetup;

        public SettingsViewModel(IViewBinder<ConfirmPopUpViewModel> confirmBinder,
            ISaveStateProvider gameStateProvider)
        {
            _confirmBinder = confirmBinder;
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

        public void SetConfirmation(ConfirmationSetup confirmSetup)
        {
            _confirmSetup = confirmSetup;
        }

        public void SetSettingsData(ISettingsData settingsData)
        {
            _settingsData.Value = settingsData;
            foreach (var section in _sections)
                section.SetSettingsData(settingsData);
        }

        public override void StartClosing()
        {
            if (!_isOpened)
                return;
            
            if (_confirmViewModel != null) // popup is opened
            {
                FLogger.LogWarning("popup is opened");
                return;
            }

            if (!_isAnyChanges.CurrentValue)
            {
                CancelUnappliedChanges();
                base.StartClosing();
                return;
            }

            _confirmViewModel = _confirmBinder.Open();
            _confirmViewModel.Setup(_confirmSetup);

            _popUpConfirmDisposable = _confirmViewModel.OnConfirmed.Take(1)
                .Subscribe(_ =>
                {
                    _popUpConfirmDisposable?.Dispose();
                    CancelUnappliedChanges();
                    base.StartClosing();
                });

            var confirmViewModel = _confirmViewModel;
            Subs.SubscribeOnce(() => _confirmViewModel = null,
                x => confirmViewModel.OnClosingCompleted += x,
                x => confirmViewModel.OnClosingCompleted -= x);
            // if cancelled: do nothing
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
            _popUpConfirmDisposable?.Dispose();
            VideoSettingsViewModel.Dispose();
            base.Dispose();
        }
    }
}