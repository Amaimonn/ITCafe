using DevKit.Utils;
using ITCafe.Data.Settings;
using R3;
using UnityEngine;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class VideoSettingsViewModel : SettingsSectionViewModel
    {
        public Observable<int> Sensitivity => _sensitivity;
        public Observable<bool> VSync => _vsync;
        public Observable<string> FPS { get; private set; }
        public Observable<int> Brightness => _brightness;
        public Observable<bool> PostProcessing => _postProcessing;
        public Observable<bool> Bloom => _bloom;
        public Observable<bool> FilmGrain => _filmGrain;
        public Observable<bool> AntiAliasing => _antiAliasing;
        public Observable<string> Resolution { get; private set; }
        public Observable<bool> Fullscreen => _fullscreen;

        private ReactiveChange<int> _sensitivity;
        private ReactiveChange<bool> _vsync;
        private ReactiveChange<int> _fps;
        private ReactiveChange<int> _brightness;
        private ReactiveChange<bool> _postProcessing;
        private ReactiveChange<bool> _bloom;
        private ReactiveChange<bool> _filmGrain;
        private ReactiveChange<bool> _antiAliasing;
        private ReactiveChange<ScreenResolution> _resolution;
        private ReactiveChange<bool> _fullscreen;

        protected override void OnBind(SettingsModel model)
        {
            _sensitivity = CreateBindedProperty(model.Sensitivity);
            _vsync = CreateBindedProperty(model.VSync);
            _fps = CreateBindedProperty(model.FPS);
            FPS = _fps.Select(FpsIntToString);
            _brightness = CreateBindedProperty(model.Brightness);
            _postProcessing = CreateBindedProperty(model.IsPostProcessingEnabled);
            _bloom = CreateBindedProperty(model.IsBloomEnabled);
            _filmGrain = CreateBindedProperty(model.IsFilmGrainEnabled);
            _antiAliasing = CreateBindedProperty(model.IsAntiAliasingEnabled);
            _resolution = CreateBindedProperty(model.ScreenResolution);
            Resolution = _resolution.Select(r => r.ToString());
            _fullscreen = CreateBindedProperty(model.Fullscreen);
        }

        public void SetSensitivity(int value)
        {
            _model.Sensitivity.Value = Mathf.Clamp(value, 1, 100);
        }

        public void SetVsync(bool value)
        {
            _model.VSync.Value = value;
        }

        public void SetFps(string value)
        {
            _model.FPS.Value = FpsStringToInt(value);
        }

        public void SetBrightness(int value)
        {
            _model.Brightness.Value = Mathf.Clamp(value, 0, 100);
        }

        public void SetPostProcessing(bool value)
        {
            _model.IsPostProcessingEnabled.Value = value;
        }

        public void SetBloom(bool value)
        {
            _model.IsBloomEnabled.Value = value;
        }

        public void SetFilmGrain(bool value)
        {
            _model.IsFilmGrainEnabled.Value = value;
        }

        public void SetAntiAliasing(bool value)
        {
            _model.IsAntiAliasingEnabled.Value = value;
        }

        public void SetResolution(string resolution)
        {
            var parts = resolution.Split('x');
            if (parts.Length == 2 && int.TryParse(parts[0], out var width) && int.TryParse(parts[1], out var height))
                _model.ScreenResolution.Value = new ScreenResolution { Width = width, Height = height };
        }

        public void SetFullscreen(bool value)
        {
            _model.Fullscreen.Value = value;
        }

        private int FpsStringToInt(string fps)
        {
            if (int.TryParse(fps, out var result))
                return result;

            return -1;
        }

        private string FpsIntToString(int fps)
        {
            // localize
            return fps == -1 ? "Максимум" : fps.ToString();
        }
    }
}