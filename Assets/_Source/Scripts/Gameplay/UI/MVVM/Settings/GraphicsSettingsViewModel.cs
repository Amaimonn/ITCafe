using DevKit.Utils;
using ITCafe.Data.Settings;
using R3;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class GraphicsSettingsViewModel : SettingsSectionViewModel
    {
        public IControlViewModel<int> Sensitivity => _sensitivity;
        public IControlViewModel<bool> VSync => _vsync;
        public IControlViewModel<string> FPS => _fps;
        public IControlViewModel<int> Brightness => _brightness;
        public IControlViewModel<bool> PostProcessing => _postProcessing;
        public IControlViewModel<bool> Bloom => _bloom;
        public IControlViewModel<bool> FilmGrain => _filmGrain;
        public IControlViewModel<bool> AntiAliasing => _antiAliasing;
        public IControlViewModel<string> Resolution => _resolution;
        public IControlViewModel<bool> Fullscreen => _fullscreen;

        private ControlViewModel<bool> _vsync;
        private ControlViewModel<int> _sensitivity;
        private ControlViewModel<string, int> _fps;
        private ControlViewModel<int> _brightness;
        private ControlViewModel<bool> _postProcessing;
        private ControlViewModel<bool> _bloom;
        private ControlViewModel<bool> _filmGrain;
        private ControlViewModel<bool> _antiAliasing;
        private ControlViewModel<string, ScreenResolution> _resolution;
        private ControlViewModel<bool> _fullscreen;

        protected override void OnBind(SettingsModel model)
        {
            _sensitivity = GetBindedControl(model.Sensitivity);
            _vsync = GetBindedControl(model.VSync);
            _fps = GetBindedControl(model.FPS, FpsIntToString, FpsStringToInt);
            _brightness = GetBindedControl(model.Brightness);
            _postProcessing = GetBindedControl(model.IsPostProcessingEnabled);
            _bloom = GetBindedControl(model.IsBloomEnabled);
            _filmGrain = GetBindedControl(model.IsFilmGrainEnabled);
            _antiAliasing = GetBindedControl(model.IsAntiAliasingEnabled);
            _resolution = GetBindedControl(model.ScreenResolution, ScreenResolutionToString,
                StringToScreenResolution, true); // delayed
            _fullscreen = GetBindedControl(model.Fullscreen, true); // delayed

            _vsync.OnChanged.Subscribe(_fps.SetWarning) // if vsync is enabled: display fps warning
                .AddTo(_disposables);

            _postProcessing.OnChanged.Select(static x => !x) // if post-processing is disabled: disable effects
                .Subscribe(x =>
                {
                    _bloom.SetWarning(x);
                    _filmGrain.SetWarning(x);
                }).AddTo(_disposables);
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