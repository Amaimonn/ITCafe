using ITCafe.Data.Settings;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class SoundSettingsViewModel : SettingsSectionViewModel
    {
        public ISettingControlViewModel<int> MusicVolume => _musicVolume;
        public ISettingControlViewModel<int> SfxVolume => _sfxVolume;

        private SettingControlViewModel<int> _musicVolume;
        private SettingControlViewModel<int> _sfxVolume;

        protected override void OnBind(SettingsModel model)
        {
            _musicVolume = CreateBindedProperty(model.MusicVolume);
            _sfxVolume = CreateBindedProperty(model.SfxVolume);
        }

        public void SetMusicVolume(int value)
        {
            _model.MusicVolume.Value = value;
        }

        public void SetSfxVolume(int value)
        {
            _model.SfxVolume.Value = value;
        }
    }
}