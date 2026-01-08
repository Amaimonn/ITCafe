using DevKit.Utils;
using ITCafe.Data.Settings;
using ObservableCollections;
using R3;
using UnityEngine;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class VideoSettingsViewModel : SettingsSectionViewModel
    {
        public ISettingControlViewModel<int> Sensitivity => _sensitivity;
        public ISettingControlViewModel<bool> VSync => _vsync;
        public ISettingControlViewModel<string> FPS => _fps;
        public ISettingControlViewModel<int> Brightness => _brightness;
        public ISettingControlViewModel<bool> PostProcessing => _postProcessing;
        public ISettingControlViewModel<bool> Bloom => _bloom;
        public ISettingControlViewModel<bool> FilmGrain => _filmGrain;
        public ISettingControlViewModel<bool> AntiAliasing => _antiAliasing;
        public ISettingControlViewModel<string> Resolution => _resolution;
        public ISettingControlViewModel<bool> Fullscreen => _fullscreen;

        private SettingControlViewModel<bool> _vsync;
        private SettingControlViewModel<int> _sensitivity;
        private SettingControlViewModel<string, int> _fps;
        private SettingControlViewModel<int> _brightness;
        private SettingControlViewModel<bool> _postProcessing;
        private SettingControlViewModel<bool> _bloom;
        private SettingControlViewModel<bool> _filmGrain;
        private SettingControlViewModel<bool> _antiAliasing;
        private SettingControlViewModel<string, ScreenResolution> _resolution;
        private SettingControlViewModel<bool> _fullscreen;

        protected override void OnBind(SettingsModel model)
        {
            _sensitivity = CreateBindedProperty(model.Sensitivity);
            _vsync = CreateBindedProperty(model.VSync);
            _fps = CreateBindedProperty(model.FPS, FpsIntToString, FpsStringToInt);
            _brightness = CreateBindedProperty(model.Brightness);
            _postProcessing = CreateBindedProperty(model.IsPostProcessingEnabled);
            _bloom = CreateBindedProperty(model.IsBloomEnabled);
            _filmGrain = CreateBindedProperty(model.IsFilmGrainEnabled);
            _antiAliasing = CreateBindedProperty(model.IsAntiAliasingEnabled);
            _resolution = CreateBindedProperty(model.ScreenResolution, ScreenResolutionToString,
                StringToScreenResolution, true); // delayed
            _fullscreen = CreateBindedProperty(model.Fullscreen, true); // delayed
            
            _vsync.OnChanged.Subscribe(_fps.SetWarning) // if vsync is enabled: display fps warning
                .AddTo(_disposables);
        }

        private string ScreenResolutionToString(ScreenResolution resolution)
        {
            if (resolution is { Width: >= 0, Height: >= 0 })
                return resolution.ToString();

            return "default";
        }

        private ScreenResolution StringToScreenResolution(string resolution)
        {
            var parts = resolution.Split('x');
            if (parts.Length == 2 && int.TryParse(parts[0], out var width) && int.TryParse(parts[1], out var height))
                return new ScreenResolution { Width = width, Height = height };
            else
                return new ScreenResolution { Width = -1, Height = -1 };
        }

        private int FpsStringToInt(string fps)
        {
            if (int.TryParse(fps, out var result))
                return result;
            FLogger.Log("Can`t parse fps");
            return -1;
        }

        private string FpsIntToString(int fps)
        {
            // localize
            return fps == -1 ? "max" : fps.ToString();
        }
    }
}