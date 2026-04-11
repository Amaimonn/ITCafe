using DevKit.UI.MVVM;
using DevKit.UI.MVVM.Bases;
using Inui.UI.MVVM.Settings;
using UnityEngine;
using R3;
using VContainer;

namespace ITCafe.UI.MVVM
{
    public class MainMenuViewModel : ScreenViewModel
    {
        private readonly IViewBinder<CampaignViewModel> _campaignBinder;
        private readonly IViewBinder<SettingsViewModel> _settingsBinder;
        private readonly IViewBinder<CreditsViewModel> _creditsBinder;
        private readonly Subject<Unit> _exitSubject;

        public MainMenuViewModel(IViewBinder<CampaignViewModel> campaignBinder,
            IViewBinder<SettingsViewModel> settingsBinder,
            IViewBinder<CreditsViewModel> creditsBinder,
            [Key(Constants.START_MISSION_SIGNAL)] Subject<Unit> exitSubject)
        {
            _campaignBinder = campaignBinder;
            _settingsBinder = settingsBinder;
            _creditsBinder = creditsBinder;
            _exitSubject = exitSubject;
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

        public void OpenCredits()
        {
            _creditsBinder.Open();
        }
    }
}