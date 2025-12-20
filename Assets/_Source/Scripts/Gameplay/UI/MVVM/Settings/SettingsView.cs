using System;
using DevKit.UI.MVVM.Bases;
using Inui.UI.MVVM.Settings;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class SettingsView : ScreenToolkitAttach<SettingsViewModel>
    {
        [Header("UIElements")]
        [SerializeField] private string _applyButtonName;
        [SerializeField] private string _cancelChangesButtonName;
        [SerializeField] private string _sectionsRootClass;

        [SerializeField] private string _settingBarLabelClass;
        [SerializeField] private string _settingBarBackgroundClass;
        [SerializeField] private VisualTreeAsset _sliderSettingBarAsset;
        [SerializeField] private VisualTreeAsset _toggleSettingBarAsset;

        private Button _applyButton;
        private Button _cancelChangesButton;

        private SliderInt _sensitivitySlider;
        private DropdownField _fpsDropdown;
        private Toggle _vSyncToggle;

        protected override void OnInit()
        {
            _applyButton = Root.Q<Button>(name: _applyButtonName);
            _cancelChangesButton = Root.Q<Button>(name: _cancelChangesButtonName);

            _sensitivitySlider = Root.Q<SliderInt>("SensitivitySlider");
            _fpsDropdown = Root.Q<DropdownField>("FPSDropdown");
            _vSyncToggle = Root.Q<Toggle>("VSyncToggle");
        }

        protected override void OnBind(SettingsViewModel viewModel)
        {
            base.OnBind(viewModel);
            var sectionViewModel = viewModel.GeneralSectionViewModel;

            BindSliderInt(_sensitivitySlider, sectionViewModel.SetSensitivity, sectionViewModel.Sensitivity);
            BindToggle(_vSyncToggle, sectionViewModel.SetVsync, sectionViewModel.VSync);
            BindDropdown(_fpsDropdown, x => sectionViewModel.SetFps(int.Parse(x)),
                sectionViewModel.FPS.Select(x => x.ToString()));
            
            _applyButton.RegisterCallback<ClickEvent>(ApplyChanges);
            _cancelChangesButton.RegisterCallback<ClickEvent>(CancelChanges);

            ViewModel.IsAnyChanges.Subscribe(x =>
            {
                _applyButton.SetEnabled(x);
                _cancelChangesButton.SetEnabled(x);
            });
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

        private void ApplyChanges(ClickEvent clickEvent)
        {
            ViewModel.ApplyChanges();
        }

        private void CancelChanges(ClickEvent clickEvent)
        {
            ViewModel.CancelUnappliedChanges();
        }
    }
}