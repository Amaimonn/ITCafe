using ITCafe.Data.Settings;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class InputSectionViewModel : SettingsSectionViewModel
    {
        public IControlViewModel<int> Sensitivity => _sensitivity;
        
        private ControlViewModel<int> _sensitivity;

        protected override void OnBind(SettingsModel model)
        {
            _sensitivity = GetBindedControl(model.Sensitivity);
        }
    }
}