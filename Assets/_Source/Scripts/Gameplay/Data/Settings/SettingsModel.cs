using R3;

namespace ITCafe.Data.Settings
{
    public class SettingsModel : Model<SettingsState>
    {
        // Sound
        public readonly ReactiveProperty<int> MusicVolume;
        public readonly ReactiveProperty<int> SfxVolume;

        // Input
        public readonly ReactiveProperty<int> Sensitivity;

        // Video
        public readonly ReactiveProperty<bool> VSync;
        public readonly ReactiveProperty<int> FPS;
        public readonly ReactiveProperty<int> Brightness;
        public readonly ReactiveProperty<bool> IsPostProcessingEnabled;
        public readonly ReactiveProperty<bool> IsBloomEnabled;
        public readonly ReactiveProperty<bool> IsFilmGrainEnabled;
        public readonly ReactiveProperty<bool> IsChromaticAberrationEnabled;
        public readonly ReactiveProperty<bool> IsAntiAliasingEnabled;
        // public readonly ReactiveProperty<int> QualityPreset;
        public readonly ReactiveProperty<ScreenResolution> ScreenResolution;
        public readonly ReactiveProperty<bool> Fullscreen;

        // Language
        public readonly ReactiveProperty<string> Language;

        public SettingsModel(SettingsState state) : base(state)
        {
            // Sound
            MusicVolume = new ReactiveProperty<int>(state.MusicVolume);
            SfxVolume = new ReactiveProperty<int>(state.SfxVolume);

            // Input
            Sensitivity = new ReactiveProperty<int>(state.Sensitivity);

            // Video
            VSync = new ReactiveProperty<bool>(state.VSync);
            FPS = new ReactiveProperty<int>(state.FPS);
            Brightness = new ReactiveProperty<int>(state.Brightness);
            IsPostProcessingEnabled = new ReactiveProperty<bool>(state.IsPostProcessingEnabled);
            IsBloomEnabled = new ReactiveProperty<bool>(state.IsBloomEnabled);
            IsFilmGrainEnabled = new ReactiveProperty<bool>(state.IsFilmGrainEnabled);
            IsChromaticAberrationEnabled = new ReactiveProperty<bool>(state.IsChromaticAberrationEnabled);
            IsAntiAliasingEnabled = new ReactiveProperty<bool>(state.IsAntiAliasingEnabled);
            // QualityPreset = new ReactiveProperty<int>(state.QualityPreset);
            ScreenResolution = new ReactiveProperty<ScreenResolution>(new ScreenResolution
            {
                Width = state.ScreenWidth,
                Height = state.ScreenHeight
            });
            Fullscreen = new ReactiveProperty<bool>(state.Fullscreen);

            // Language
            Language = new ReactiveProperty<string>(state.Language);
        }

        /// <summary>
        /// Заносит текущие значения из модели в состояние (для сохранения).
        /// </summary>
        public void ApplyToState()
        {
            // Sound
            State.MusicVolume = MusicVolume.Value;
            State.SfxVolume = SfxVolume.Value;

            // Input
            State.Sensitivity = Sensitivity.Value;

            // Video
            State.VSync = VSync.Value;
            State.FPS = FPS.Value;
            State.Brightness = Brightness.Value;
            State.IsPostProcessingEnabled = IsPostProcessingEnabled.Value;
            State.IsBloomEnabled = IsBloomEnabled.Value;
            State.IsFilmGrainEnabled = IsFilmGrainEnabled.Value;
            State.IsAntiAliasingEnabled = IsAntiAliasingEnabled.Value;
            // State.QualityPreset = QualityPreset.Value;
            State.ScreenWidth = ScreenResolution.Value.Width;
            State.ScreenHeight = ScreenResolution.Value.Height;
            State.Fullscreen = Fullscreen.Value;

            // Language
            State.Language = Language.Value;
        }
    }
}