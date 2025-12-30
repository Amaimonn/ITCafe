using System.Linq;
using DevKit.Utils;
using ITCafe.Data;
using R3;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class GeneralSectionViewModel : SettingsSectionViewModel
    {
        public Observable<int> Sensitivity => _sensitivity;
        public Observable<bool> VSync => _vsync;
        public Observable<int> FPS => _fps;

        private ReactiveChange<int> _sensitivity;
        private ReactiveChange<bool> _vsync;
        private ReactiveChange<int> _fps;

        protected override void OnBind(SettingsModel model)
        {
            _sensitivity = CreateBindedProperty(model.Sensitivity);
            _vsync = CreateBindedProperty(model.VSync);
            _fps = CreateBindedProperty(model.FPS);

            IsAnyChanges = Observable.CombineLatest(
                    _sensitivity.IsChanged,
                    _vsync.IsChanged,
                    _fps.IsChanged
                ).Select(x => x.Any(t => t == true))
                .ToReadOnlyReactiveProperty();
        }

        public void SetSensitivity(int value)
        {
            _model.Sensitivity.Value = value;
        }

        public void SetVsync(bool value)
        {
            _model.VSync.Value = value;
        }

        public void SetFps(int value)
        {
            _model.FPS.Value = value;
        }
    }
}