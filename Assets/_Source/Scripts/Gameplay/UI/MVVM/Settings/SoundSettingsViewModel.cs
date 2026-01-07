using DevKit.Utils;
using ITCafe.Data.Settings;
using R3;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class SoundSettingsViewModel : SettingsSectionViewModel
    {
        public Observable<int> MusicVolume => _musicVolume;
        public Observable<int> SfxVolume => _sfxVolume;

        private ReactiveChange<int> _musicVolume;
        private ReactiveChange<int> _sfxVolume;

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