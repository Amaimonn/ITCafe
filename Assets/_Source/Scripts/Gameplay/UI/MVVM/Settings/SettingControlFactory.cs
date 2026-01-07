using System;
using System.Linq;
using DevKit.UITK;
using ITCafe.Data.Settings;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Gameplay.UI.MVVM
{
    [Serializable]
    public class SettingControlFactory
    {
        [Space(4)]
        [SerializeField] private string _settingBarLabelName = "BarLabel";

        [Header("Arrow Menu"), Space(4)]
        [SerializeField] private VisualTreeAsset _arrowMenuBar;

        [Header("Slider Int"), Space(4)]
        [SerializeField] private VisualTreeAsset _sliderIntBarAsset;

        [Header("Toggle"), Space(4)]
        [SerializeField] private VisualTreeAsset _toggleBarAsset;

        [Header("Dropdown"), Space(4)]
        [SerializeField] private VisualTreeAsset _dropdownBarAsset;
        
        public Tab AddBindedTab(VisualElement sectionsRoot, string labelText, Action<Tab> selectAction)
        {
            var tab = new Tab();
            
            // tab.LocalizeLabel(Tables.SETTINGS, labelEntry);
            tab.selected += selectAction;
            sectionsRoot.Add(tab);

            return tab;
        }
        
        public void AddBindedSliderInt(ISliderSettingData<int> sliderSettingData, VisualElement parentSection,
            Action<int> onInput, Observable<int> observable)
        {
            var slider = CreateSliderInt(sliderSettingData, parentSection);
            BindSliderInt(slider, onInput, observable);
        }

        public SliderInt CreateSliderInt(ISliderSettingData<int> sliderSettingData, VisualElement parentSection)
        {
            var settingBar = _sliderIntBarAsset.CloneTree();
            parentSection.Add(settingBar);

            var slider = settingBar.Q<SliderInt>();
            slider.lowValue = sliderSettingData.MinValue;
            slider.highValue = sliderSettingData.MaxValue;

            // var label = settingBar.Q<Label>(className: _settingBarLabelClass);
            // label.LocalizeText(Tables.SETTINGS, sliderSettingData.Label);

            return slider;
        }

        public void BindSliderInt(SliderInt slider, Action<int> onInput, Observable<int> observable)
        {
            slider.RegisterCallback<ChangeEvent<int>>(e => onInput(e.newValue));
            observable.Subscribe(x => slider.value = x);
        }

        public void AddBindedToggle(IToggleSettingData toggleSettingData, VisualElement parentSection,
            Action<bool> onInput, Observable<bool> observable)
        {
            var toggle = CreateToggle(toggleSettingData, parentSection);
            BindToggle(toggle, onInput, observable);
        }

        public Toggle CreateToggle(IToggleSettingData toggleSettingData, VisualElement parentSection)
        {
            var settingBar = _toggleBarAsset.CloneTree();
            parentSection.Add(settingBar);

            var toggle = settingBar.Q<Toggle>();

            // var label = settingBar.Q<Label>(className: _settingBarLabelClass);
            // label.LocalizeText(Tables.SETTINGS, toggleSettingData.Label);

            return toggle;
        }

        public void BindToggle(Toggle toggle, Action<bool> onInput, Observable<bool> observable)
        {
            toggle.RegisterCallback<ChangeEvent<bool>>(e => onInput(e.newValue));
            observable.Subscribe(x => toggle.value = x);
        }

        public void AddBindedDropdown(IOptionsSettingData dropdownSettingData, VisualElement parentSection,
            Action<string> onInput, Observable<string> observable)
        {
            var dropdown = CreateDropdown(dropdownSettingData, parentSection);
            BindDropdown(dropdown, onInput, observable);
        }

        public DropdownField CreateDropdown(IOptionsSettingData dropdownSettingData, VisualElement parentSection)
        {
            var settingBar = _dropdownBarAsset.CloneTree();
            parentSection.Add(settingBar);

            // localize label

            var dropdown = settingBar.Q<DropdownField>();
            dropdown.choices = dropdownSettingData.Options.ToList(); // localize options if necessary

            return dropdown;
        }

        public void BindDropdown(DropdownField dropdown, Action<string> onInput, Observable<string> observable)
        {
            dropdown.RegisterValueChangedCallback(e => onInput(e.newValue));
            observable.Subscribe(x => dropdown.value = x);
        }

        public void AddBindedArrowMenu(IOptionsSettingData arrowMenuSettingData, VisualElement parentSection,
            Action<string> onInput, Observable<string> observable)
        {
            var arrowMenu = CreateArrowMenu(arrowMenuSettingData, parentSection);
            BindArrowMenu(arrowMenu, onInput, observable);
        }

        public ArrowMenuString CreateArrowMenu(IOptionsSettingData arrowMenuSettingData, VisualElement parentSection)
        {
            var settingBar = _arrowMenuBar.CloneTree();
            parentSection.Add(settingBar);

            // var label = settingBar.Q<Label>(className: _settingBarLabelClass);
            // label.LocalizeText(Tables.SETTINGS, arrowsSettingData.Label);

            var arrowMenuInt = settingBar.Q<ArrowMenuString>();
            arrowMenuInt.Options = arrowMenuSettingData.Options;

            return arrowMenuInt;
        }

        public void BindArrowMenu<T>(ArrowMenu<T> arrowsMenu, Action<T> onInput, Observable<T> observable)
        {
            arrowsMenu.OnValueChanged += onInput;
            observable.Subscribe(x => arrowsMenu.Value = x);
        }
    }
}