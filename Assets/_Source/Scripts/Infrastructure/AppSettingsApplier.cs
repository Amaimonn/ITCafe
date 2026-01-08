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
            var disposables =  new CompositeDisposable();
            model.VSync.Subscribe(x => QualitySettings.vSyncCount = x ? 1 : 0)
                .AddTo(disposables);

            model.FPS.Subscribe(x =>
            {
                Application.targetFrameRate = x;
            }).AddTo(disposables);
            
            return disposables;
        }
    }
}