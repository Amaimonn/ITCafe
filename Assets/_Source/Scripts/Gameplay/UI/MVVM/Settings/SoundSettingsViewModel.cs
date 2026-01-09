using ITCafe.Data.Settings;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class SoundSettingsViewModel : SettingsSectionViewModel
    {
        public IControlViewModel<int> MusicVolume => _musicVolume;
        public IControlViewModel<int> SfxVolume => _sfxVolume;

        private ControlViewModel<int> _musicVolume;
        private ControlViewModel<int> _sfxVolume;

        protected override void OnBind(SettingsModel model)
        {
            _musicVolume = GetBindedControl(model.MusicVolume);
            _sfxVolume = GetBindedControl(model.SfxVolume);
        }
    }
}