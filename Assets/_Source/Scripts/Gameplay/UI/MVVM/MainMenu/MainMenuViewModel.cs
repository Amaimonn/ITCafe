using DevKit.UI.MVVM;
using DevKit.UI.MVVM.Bases;
using Inui.UI.MVVM.Settings;
using UnityEngine;
using R3;
using VContainer;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class MainMenuViewModel : ScreenViewModel
    {
        private readonly Subject<Unit> _exitSubject;
        private readonly IViewBinder<SettingsViewModel> _settingsBinder;

        public MainMenuViewModel([Key(Constants.MAIN_MENU_EXIT_SIGNAL)] Subject<Unit> exitSubject,
            IViewBinder<SettingsViewModel> settingsBinder)
        {
            _exitSubject = exitSubject;
            _settingsBinder = settingsBinder;
        }

        public void StartGameplay()
        {
            Debug.Log("Start Gameplay signal in vm");
            _exitSubject.OnNext(Unit.Default);
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void OpenSettings()
        {
            _settingsBinder.Open();
        }
    }
}