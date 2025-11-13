using DevKit.UI.MVVM.Bases;
using UnityEngine;
using R3;
using VContainer;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class MainMenuViewModel : ScreenViewModel
    {
        private readonly Subject<Unit> _exitSubject;

        public MainMenuViewModel([Key(Constants.MAIN_MENU_EXIT_SIGNAL)]Subject<Unit> exitSubject)
        {
            _exitSubject = exitSubject;
        }

        public void StartGameplay()
        {
            Debug.Log("Start Gameplay signal in vm");
            _exitSubject.OnNext(Unit.Default);
        }

        // public void OpenSettings()
        // {
        //     _settingsBinder.TryBindAndOpen(out _);
        // }
    }
}