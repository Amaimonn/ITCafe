using DevKit.Utils;
using DevKit.UI.MVVM.Bases;
using ITCafe.Data;
using ITCafe.Gameplay.UI.MVVM;
using ITCafe.Infrastructure.Saves;
using R3;

namespace Inui.UI.MVVM.Settings
{
    public class SettingsViewModel : ScreenViewModel
    {
        public Observable<bool> IsAnyChanges => _isAnyChanges;
        public readonly GeneralSectionViewModel GeneralSectionViewModel;
        private readonly ISaveStateProvider _gameStateProvider;
        
        private ReadOnlyReactiveProperty<bool> _isAnyChanges;
        private SettingsModel _model;

        public SettingsViewModel(ISaveStateProvider gameStateProvider)
        {
            _gameStateProvider = gameStateProvider;
            GeneralSectionViewModel = new GeneralSectionViewModel();
        }

        public void Bind(SettingsModel model)
        {
            _model = model;
            GeneralSectionViewModel.Bind(model);

            _isAnyChanges = GeneralSectionViewModel.IsAnyChanges;
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
            GeneralSectionViewModel.ApplyChanges();
            _model.ApplyToState(); // состояние для сохранения изменяется только после подтверждения установленных настроек
            SaveSettings();
            FLogger.Log("Settings: Applied");
        }

        /// <summary>
        /// Отменяет несохраённые изменения
        /// </summary>
        public void CancelUnappliedChanges()
        {
            GeneralSectionViewModel.CancelChanges();
            FLogger.Log("Settings: Cancelled");
        }

        private void SaveSettings()
        {
            _gameStateProvider.SaveAll();
        }

        public override void Dispose()
        {
            GeneralSectionViewModel.Dispose();
            base.Dispose();
        }
    }
}