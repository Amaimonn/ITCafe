using DevKit.Utils;
using DevKit.UI.MVVM.Bases;
using ITCafe.Gameplay.Data;
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
        private readonly ReadOnlyReactiveProperty<bool> _isAnyChanges;
        private readonly SettingsModel _model;

        public SettingsViewModel(SettingsModel model, ISaveStateProvider gameStateProvider)
        {
            _model = model;
            _gameStateProvider = gameStateProvider;
            GeneralSectionViewModel = new GeneralSectionViewModel(model);

            _isAnyChanges = GeneralSectionViewModel.IsAnyChanges;
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

        public void Dispose()
        {
            GeneralSectionViewModel.Dispose();
        }
    }
}