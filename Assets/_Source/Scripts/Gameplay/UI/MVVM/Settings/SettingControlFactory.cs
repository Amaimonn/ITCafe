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
        [SerializeField] private string _tabClass = "settings__section-tab";
        [SerializeField] private string _settingBarLabelName = "SettingBarLabel";

        [Header("Arrow Menu"), Space(4)]
        [SerializeField] private VisualTreeAsset _arrowMenuBar;

        [Header("Slider Int"), Space(4)]
        [SerializeField] private VisualTreeAsset _sliderIntBarAsset;

        [Header("Toggle"), Space(4)]
        [SerializeField] private VisualTreeAsset _toggleBarAsset;

        [Header("Dropdown"), Space(4)]
        [SerializeField] private VisualTreeAsset _dropdownBarAsset;

        public TabScrollView AddBindedTab(VisualElement sectionsRoot, string labelText, Action<Tab> selectAction)
        {
            var tab = new TabScrollView();

            tab.AddToClassList(_tabClass);
            tab.label = labelText;
            
            tab.selected += x =>
            {
                selectAction(x);
                tab.ScrollToTop(); // TODO: check in case of equality
            };
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
            var settingBar = CreateBar(_sliderIntBarAsset, sliderSettingData.Label, parentSection);
            var slider = settingBar.Q<SliderInt>();
            
            slider.lowValue = sliderSettingData.MinValue;
            slider.highValue = sliderSettingData.MaxValue;

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
            var settingBar = CreateBar(_toggleBarAsset, toggleSettingData.Label, parentSection);
            var toggle = settingBar.Q<Toggle>();

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
            var settingBar = CreateBar(_dropdownBarAsset, dropdownSettingData.Label, parentSection);
            var dropdown = settingBar.Q<DropdownField>();
            
            // localize options if necessary
            // var overrideOptions= dropdownSettingData.OverrideDisplayOptions;
            // var options = dropdownSettingData.Options;
            
            // if (overrideOptions is { Length: > 0 } && overrideOptions.Length == options.Length)
            //     dropdown.choices = dropdownSettingData.OverrideDisplayOptions.ToList();
            
            dropdown.choices = dropdownSettingData.Options.ToList();

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
            var settingBar = CreateBar(_arrowMenuBar, arrowMenuSettingData.Label, parentSection);
            var arrowMenuInt = settingBar.Q<ArrowMenuString>();
            
            arrowMenuInt.Options = arrowMenuSettingData.Options;

            return arrowMenuInt;
        }

        private VisualElement CreateBar(VisualTreeAsset asset, string labelText, VisualElement parent)
        {
            var settingBar = asset.CloneTree();
            parent.Add(settingBar);

            var label = settingBar.Q<Label>(name: _settingBarLabelName);
            label.text = labelText;

            return settingBar;
        }

        public void BindArrowMenu<T>(ArrowMenu<T> arrowsMenu, Action<T> onInput, Observable<T> observable)
        {
            arrowsMenu.OnValueChanged += onInput;
            observable.Subscribe(x => arrowsMenu.Value = x);
        }
    }
}