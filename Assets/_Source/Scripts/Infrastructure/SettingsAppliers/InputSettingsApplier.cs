using System;
using ITCafe.Data.Settings;
using R3;
using Unity.Cinemachine;
using UnityEngine;

namespace ITCafe
{
    public class InputSettingsApplier : SettingsApplier
    {
        private readonly CinemachineInputAxisController _cinemachineInputAxisController;

        public InputSettingsApplier(CinemachineInputAxisController cinemachineInputAxisController)
        {
            _cinemachineInputAxisController = cinemachineInputAxisController;
        }
        
        public override IDisposable BindSettings(SettingsModel settingsModel)
        {
            var disposables = new CompositeDisposable
            {
                settingsModel.Sensitivity.Subscribe(x =>
                {
                    var newValue = x <= 50f
                        ? Mathf.Lerp(0.2f, 1f, Mathf.InverseLerp(1f, 50f, x))
                        : Mathf.Lerp(1f, 5f, Mathf.InverseLerp(50f, 100f, x));

                    foreach (var c in _cinemachineInputAxisController.Controllers)
                    {
                        c.Input.Gain = c.Name switch
                        {
                            "Look X (Pan)" => newValue,
                            "Look Y (Tilt)" => -newValue,
                            _ => c.Input.Gain
                        };
                    }
                })
            };
            
            return disposables;
        }
    }
}