using System;
using System.Collections.Generic;
using DevKit.Utils;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using R3;
using ITCafe.Data.Settings;

namespace ITCafe
{
    public class PostProcessingSettingsApplier : SettingsApplier
    {
        private readonly Volume _volume;

        public PostProcessingSettingsApplier(Volume volume)
        {
            _volume = volume;
        }

        public override IDisposable BindSettings(SettingsModel settingsModel)
        {
            if (_volume == null)
            {
                FLogger.LogWarning("Volume is null");
                return Disposable.Empty;
            }

            var disposables = new CompositeDisposable();
            var optionalEffects = new List<VolumeComponent>();

            if (_volume.profile.TryGet<Bloom>(out var bloom))
            {
                optionalEffects.Add(bloom);
                settingsModel.IsBloomEnabled
                    .Subscribe(x => bloom.active = x)
                    .AddTo(disposables);
            }

            if (_volume.profile.TryGet<FilmGrain>(out var filmGrain))
            {
                optionalEffects.Add(filmGrain);
                settingsModel.IsFilmGrainEnabled
                    .Subscribe(x => filmGrain.active = x)
                    .AddTo(disposables);
            }
            
            if (_volume.profile.TryGet<ChromaticAberration>(out var chromaticAberration))
            {
                optionalEffects.Add(chromaticAberration);
                settingsModel.IsChromaticAberrationEnabled
                    .Subscribe(x => chromaticAberration.active = x)
                    .AddTo(disposables);
            }

            if (_volume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
            {
                settingsModel.Brightness
                    .Subscribe(x => colorAdjustments.postExposure.value = x / 50f)
                    .AddTo(disposables);
            }

            if (optionalEffects.Count > 0)
            {
                settingsModel.IsPostProcessingEnabled
                    .Subscribe(x =>
                    {
                        foreach (var effect in optionalEffects)
                            effect.active = x;
                    })
                    .AddTo(disposables);
            }
            
            return disposables;
        }
    }
}