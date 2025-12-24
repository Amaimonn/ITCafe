using DevKit.UI.MVVM;
using DevKit.UI.MVVM.Bases;
using Inui.UI.MVVM.Settings;
using R3;
using UnityEngine;
using VContainer;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class PauseViewModel : ScreenViewModel
    {
        private readonly Subject<Unit> _exitToMenuSignal;
        private readonly Subject<Unit> _restartSignal;
        private readonly InputService _inputService;
        private readonly IViewBinder<SettingsViewModel> _settingsBinder;
        
        public PauseViewModel([Key(Constants.GAMEPLAY_EXIT_SIGNAL)] Subject<Unit> exitToMenuSignal,
            [Key(Constants.RESTART_GAMEPLAY_SIGNAL)] Subject<Unit> restartSignal, InputService inputService,
            IViewBinder<SettingsViewModel> settingsBinder)
        {
            _exitToMenuSignal = exitToMenuSignal;
            _restartSignal = restartSignal;
            _inputService = inputService;
            _settingsBinder = settingsBinder;
        }

        public override void Open()
        {
            _inputService.SetInputEnabled(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            base.Open();
        }

        public override void CompleteClosing()
        {
            _inputService.SetInputEnabled(true);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            base.CompleteClosing();
        }

        public void ExitToMenu()
        {
            _exitToMenuSignal.OnNext(Unit.Default);
        }

        public void OpenSettings()
        {
            _settingsBinder.Open();
        }
        
        public void Restart()
        {
            _restartSignal.OnNext(Unit.Default);
        }
    }
}

