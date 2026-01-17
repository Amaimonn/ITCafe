using System;
using ITCafe.Data.Settings;
using R3;
using UnityEngine;

namespace ITCafe
{
    public class AppSettingsApplier
    {
        public IDisposable BindSettings(SettingsModel model)
        {
            var disposables = new CompositeDisposable()
            {
                model.VSync.Subscribe(x => QualitySettings.vSyncCount = x ? 1 : 0),
                model.IsAntiAliasingEnabled.Subscribe(x => QualitySettings.antiAliasing = x ? 1 : 0),
                model.ScreenResolution.Subscribe(x =>
                {
                    if (x is { Width: > 0, Height: > 0 })
                        Screen.SetResolution(x.Width, x.Height, Screen.fullScreen);
                    else
                        Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, Screen.fullScreen);
                }),
                model.Fullscreen.Subscribe(x => Screen.fullScreen = x),
                model.FPS.Subscribe(x => Application.targetFrameRate = x),
            };

            return disposables;
        }
    }
}