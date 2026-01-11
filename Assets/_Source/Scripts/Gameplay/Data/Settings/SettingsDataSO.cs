using UnityEngine;

namespace ITCafe.Data.Settings
{
    [CreateAssetMenu(fileName = "SettingsDataSO", menuName = "Scriptable Objects/Settings/SettingsDataSO")]
    public class SettingsDataSO : ScriptableObject, ISettingsData
    {
        // Sound settings
        public ISliderSettingData<int> MusicVolumeData => _musicVolumeSliderDataSO;
        public ISliderSettingData<int> SfxVolumeData => _sfxVolumeSliderDataSO;
        
        // Input settings
        public ISliderSettingData<int> SensitivityData => _sensitivitySliderDataSO;

        // Video settings
        public IOptionsSettingData FpsData => _fpsOptionsDataSO;
        public IToggleSettingData VSyncData => _vsyncToggleDataSO;
        public ISliderSettingData<int> BrightnessData => _brightnessSliderDataSO;
        public IToggleSettingData IsPostProcessingEnabledData => _isPostProcessingEnabledToggleDataSO;
        public IToggleSettingData IsBloomEnabledData => _isBloomEnabledToggleDataSO;
        public IToggleSettingData IsFilmGrainEnabledData => _isFilmGrainEnabledToggleDataSO;
        public IToggleSettingData IsAntiAliasingEnabledData => _isAntiAliasingEnabledToggleDataSO;
        public IOptionsSettingData QualityPresetData => _qualityPresetDataSO;
        public IOptionsSettingData ResolutionData => _resolutionDataSO;
        public IToggleSettingData FullscreenData => _fullscreenToggleDataSO;
        
        // Language settings
        public IOptionsSettingData LanguageData => _languageDataSO;
        
        [field: SerializeField] public string SoundSectionLabel { get; private set; }
        [field: SerializeField] public string VideoSectionLabel { get; private set; }
        [field: SerializeField] public string InputSectionLabel { get; private set; }
        [field: SerializeField] public string LanguageSectionLabel { get; private set; }

        [field: SerializeField] public Sprite SoundIcon { get; private set; }
        [field: SerializeField] public Sprite GraphicsIcon { get; private set; }
        [field: SerializeField] public Sprite InputIcon { get; private set; }
        [field: SerializeField] public Sprite LanguageIcon { get; private set; }
        
        [Header("Sound Settings")]
        [SerializeField] private SliderSettingDataSO<int> _musicVolumeSliderDataSO;
        [SerializeField] private SliderSettingDataSO<int> _sfxVolumeSliderDataSO;
        
        [Header("Input Settings")]
        [SerializeField] private SliderSettingDataSO<int> _sensitivitySliderDataSO;
        
        [Header("Video Settings")]
        [SerializeField] private OptionsSettingDataSO _fpsOptionsDataSO;
        [SerializeField] private ToggleSettingDataSO _vsyncToggleDataSO;
        [SerializeField] private SliderSettingDataSO<int> _brightnessSliderDataSO;
        [SerializeField] private ToggleSettingDataSO _isPostProcessingEnabledToggleDataSO;
        [SerializeField] private ToggleSettingDataSO _isBloomEnabledToggleDataSO;
        [SerializeField] private ToggleSettingDataSO _isFilmGrainEnabledToggleDataSO;
        [SerializeField] private ToggleSettingDataSO _isAntiAliasingEnabledToggleDataSO;
        [SerializeField] private OptionsSettingDataSO _qualityPresetDataSO;
        [SerializeField] private OptionsSettingDataSO _resolutionDataSO;
        [SerializeField] private ToggleSettingDataSO _fullscreenToggleDataSO;
        
        [Header("Language Settings")]
        [SerializeField] private OptionsSettingDataSO _languageDataSO;
    }
}