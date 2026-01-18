using ITCafe.Data.Settings;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class LanguageSectionViewModel : SettingsSectionViewModel
    {
        public IControlViewModel<string> Language => _language;
        
        private ControlViewModel<string> _language;

        protected override void OnBind(SettingsModel model)
        {
            _language = GetBindedControl(model.Language);
        }
    }
}