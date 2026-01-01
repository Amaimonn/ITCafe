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
        private readonly IViewBinder<CampaignViewModel> _campaignBinder;

        public MainMenuViewModel([Key(Constants.START_MISSION_SIGNAL)] Subject<Unit> exitSubject,
            IViewBinder<SettingsViewModel> settingsBinder, IViewBinder<CampaignViewModel> campaignBinder)
        {
            _exitSubject = exitSubject;
            _settingsBinder = settingsBinder;
            _campaignBinder = campaignBinder;
        }

        public void StartGameplay()
        {
            Debug.Log("Start Gameplay signal in vm");
            _campaignBinder.Open();
            // _exitSubject.OnNext(Unit.Default);
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