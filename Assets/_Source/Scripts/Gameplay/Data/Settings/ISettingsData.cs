using UnityEngine;

namespace ITCafe.Data.Settings
{
    public interface ISettingsData
    {
        public string SoundSectionLabel { get; }
        public string VideoSectionLabel { get; }
        public string InputSectionLabel { get; }
        public string LanguageSectionLabel { get; }

        public Sprite SoundIcon { get; }
        public Sprite GraphicsIcon { get; }
        public Sprite InputIcon { get; }
        public Sprite LanguageIcon { get; }
        
        public ISliderSettingData<int> MusicVolumeData { get; }
        public ISliderSettingData<int> SfxVolumeData { get; }
        
        public ISliderSettingData<int> SensitivityData { get; }
        public IOptionsSettingData FpsData { get; }
        public IToggleSettingData VSyncData { get; }
        public ISliderSettingData<int> BrightnessData { get; }
        public IToggleSettingData IsPostProcessingEnabledData { get; }
        public IToggleSettingData IsBloomEnabledData { get; }
        public IToggleSettingData IsFilmGrainEnabledData { get; }
        public IToggleSettingData IsAntiAliasingEnabledData { get; }
        public IOptionsSettingData ResolutionData { get; }
        public IToggleSettingData FullscreenData { get; }
        
        public IOptionsSettingData LanguageData { get; }
    }
}