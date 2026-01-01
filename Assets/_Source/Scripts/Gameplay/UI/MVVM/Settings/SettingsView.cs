using System;
using DevKit.UI.MVVM.Bases;
using DevKit.UITK;
using DevKit.Utils;
using Inui.UI.MVVM.Settings;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class SettingsView : AttachableToolkitScreen<SettingsViewModel>
    {
        [Header("UIElements")]
        [SerializeField] private string _applyButtonName = "ApplyButton";
        [SerializeField] private string _cancelChangesButtonName = "CancelChangesButton";
        [SerializeField] private string _closeButtonName = "CloseButton";

        private Button _applyButton;
        private Button _cancelChangesButton;
        private Button _closeButton;

        private SliderInt _sensitivitySlider;
        private DropdownField _fpsDropdown;
        private Toggle _vSyncToggle;

        protected override void OnInit()
        {
            _applyButton = Root.Q<Button>(name: _applyButtonName);
            _cancelChangesButton = Root.Q<Button>(name: _cancelChangesButtonName);
            _closeButton = Root.Q<Button>(name: _closeButtonName);

            _sensitivitySlider = Root.Q<KitSliderInt>("SensitivitySlider");
            _fpsDropdown = Root.Q<DropdownField>("FPSDropdown");
            _vSyncToggle = Root.Q<Toggle>("VSyncToggle");
        }

        protected override void OnBind(SettingsViewModel viewModel)
        {
            base.OnBind(viewModel);
            var sectionViewModel = viewModel.GeneralSectionViewModel;

            BindSliderInt(_sensitivitySlider, sectionViewModel.SetSensitivity, sectionViewModel.Sensitivity);
            BindToggle(_vSyncToggle, sectionViewModel.SetVsync, sectionViewModel.VSync);
            BindDropdown(_fpsDropdown, x => sectionViewModel.SetFps(SensitivityStringToInt(x)),
                sectionViewModel.FPS.Select(SensitivityIntToString));

            _applyButton.RegisterCallback<ClickEvent>(ApplyChanges);
            _cancelChangesButton.RegisterCallback<ClickEvent>(CancelChanges);
            _closeButton.SubscribeCallbackOnce<ClickEvent>(OnCloseClicked)
                .AddTo(_disposables);

            ViewModel.IsAnyChanges.Subscribe(x =>
            {
                _applyButton.SetEnabled(x);
                _cancelChangesButton.SetEnabled(x);
            }).AddTo(_disposables);;
        }

        private void BindSliderInt(SliderInt slider, Action<int> update, Observable<int> observable)
        {
            slider.RegisterCallback<ChangeEvent<int>>(e => update(e.newValue));
            observable.Subscribe(x => slider.value = x);
        }

        private void BindToggle(Toggle toggle, Action<bool> update, Observable<bool> observable)
        {
            toggle.RegisterCallback<ChangeEvent<bool>>(e => update(e.newValue));
            observable.Subscribe(x => toggle.value = x);
        }

        private void BindDropdown(DropdownField toggle, Action<string> update, Observable<string> observable)
        {
            toggle.RegisterValueChangedCallback(e => update(e.newValue));
            observable.Subscribe(x => toggle.value = x);
        }

        private int SensitivityStringToInt(string sensitivity)
        {
            if (int.TryParse(sensitivity, out var result))
                return result;
            return -1;
        }

        private string SensitivityIntToString(int sensitivity)
        {
            return sensitivity == -1 ? "Максимум" : sensitivity.ToString();
        }

        private void ApplyChanges(ClickEvent clickEvent)
        {
            ViewModel.ApplyChanges();
        }

        private void CancelChanges(ClickEvent clickEvent)
        {
            ViewModel.CancelUnappliedChanges();
        }
        
        private void OnCloseClicked(ClickEvent _)
        {
            ViewModel.StartClosing();
        }
    }
}