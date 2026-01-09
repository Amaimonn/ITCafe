using System;
using System.Collections.Generic;
using System.Linq;
using DevKit.UITK;
using DevKit.Utils;
using ITCafe.Data.Settings;
using ITCafe.Gameplay.UI.Custom;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Gameplay.UI.MVVM
{
    [Serializable]
    public class SettingControlFactory : IDisposable
    {
        [Header("Tab")]
        [SerializeField] private string _tabClass = "settings__section";
        [SerializeField] private string _tabScrollViewClass = "cafe__scroll-view";
        [SerializeField] private string _settingBarLabelName = "SettingBarLabel";

        [Header("Info Container")]
        [SerializeField] private string _infoEntryClass = "settings__info-entry";
        [SerializeField] private string _descriptionClass = "settings__info-description";
        [SerializeField] private string _warningDescriptionClass = "settings__info-warning";

        [Header("Tab Button"), Space(4)]
        [SerializeField] private VisualTreeAsset _tabButtonAsset;

        [Header("Arrow Menu"), Space(4)]
        [SerializeField] private VisualTreeAsset _arrowMenuBarAsset;

        [Header("Slider Int"), Space(4)]
        [SerializeField] private VisualTreeAsset _sliderIntBarAsset;

        [Header("Toggle"), Space(4)]
        [SerializeField] private VisualTreeAsset _toggleBarAsset;

        [Header("Dropdown"), Space(4)]
        [SerializeField] private VisualTreeAsset _dropdownBarAsset;

        private VisualElement _sectionsContainer;
        private VisualElement _buttonsContainer;
        private VisualElement _controlInfoContainer;
        private CompositeDisposable _disposables;

        public void Init(VisualElement sectionsContainer, VisualElement buttonsContainer,
            VisualElement controlInfoContainer)
        {
            _sectionsContainer = sectionsContainer;
            _buttonsContainer = buttonsContainer;
            _controlInfoContainer = controlInfoContainer;
            _disposables = new();
        }

        public TabEntry AddBindedTab(string labelText, EventCallback<ClickEvent> onButtonClicked)
        {
            var scrollView = new ScrollView();
            scrollView.AddToClassList(_tabClass);
            scrollView.AddToClassList(_tabScrollViewClass);
            _sectionsContainer.Add(scrollView);

            var tabButtonTemplate = _tabButtonAsset.CloneTree();
            _buttonsContainer.Add(tabButtonTemplate);

            var tabButton = tabButtonTemplate.Q<Button>();
            tabButton.RegisterCallback<ClickEvent>(onButtonClicked);
            tabButton.text = labelText;

            return new TabEntry { Button = tabButton, ScrollView = scrollView };
        }

        public void AddBindedSliderInt(ISliderSettingData<int> sliderSettingData, VisualElement parentSection,
            IControlViewModel<int> controlViewModel)
        {
            var settingBar = CreateBar(_sliderIntBarAsset, sliderSettingData.Label, parentSection);
            var slider = settingBar.Q<SliderInt>();

            slider.lowValue = sliderSettingData.MinValue;
            slider.highValue = sliderSettingData.MaxValue;

            BindSliderInt(slider, controlViewModel);
            BindBar(settingBar, slider, controlViewModel, sliderSettingData);
        }

        private void BindSliderInt(SliderInt slider, IControlViewModel<int> controlViewModel)
        {
            slider.RegisterCallback<ChangeEvent<int>>(e => controlViewModel.SetValue(e.newValue));
            controlViewModel.OnChanged.Subscribe(x => slider.value = x)
                .AddTo(_disposables);
        }

        public void AddBindedToggle(IToggleSettingData toggleSettingData, VisualElement parentSection,
            IControlViewModel<bool> controlViewModel)
        {
            var settingBar = CreateBar(_toggleBarAsset, toggleSettingData.Label, parentSection);
            var toggle = settingBar.Q<Toggle>();

            BindToggle(toggle, controlViewModel);
            BindBar(settingBar, toggle, controlViewModel, toggleSettingData);
        }

        private void BindToggle(Toggle toggle, IControlViewModel<bool> controlViewModel)
        {
            toggle.RegisterCallback<ChangeEvent<bool>>(e => controlViewModel.SetValue(e.newValue));
            controlViewModel.OnChanged.Subscribe(x => toggle.value = x)
                .AddTo(_disposables);
        }

        public void AddBindedDropdown(IOptionsSettingData dropdownSettingData, VisualElement parentSection,
            IControlViewModel<string> controlViewModel)
        {
            var settingBar = CreateBar(_dropdownBarAsset, dropdownSettingData.Label, parentSection);
            var dropdown = settingBar.Q<DropdownField>();

            // localize options if necessary
            // var overrideOptions= dropdownSettingData.OverrideDisplayOptions;
            // var options = dropdownSettingData.Options;

            // if (overrideOptions is { Length: > 0 } && overrideOptions.Length == options.Length)
            //     dropdown.choices = dropdownSettingData.OverrideDisplayOptions.ToList();

            dropdown.choices = dropdownSettingData.Options.ToList();

            BindDropdown(dropdown, controlViewModel);
            BindBar(settingBar, dropdown, controlViewModel, dropdownSettingData);
        }

        private void BindDropdown(DropdownField dropdown, IControlViewModel<string> controlViewModel)
        {
            dropdown.RegisterValueChangedCallback(e => controlViewModel.SetValue(e.newValue));
            controlViewModel.OnChanged.Subscribe(x => dropdown.value = x)
                .AddTo(_disposables);
        }

        public void AddBindedArrowMenu(IOptionsSettingData arrowMenuSettingData, VisualElement parentSection,
            IControlViewModel<string> controlViewModel)
        {
            var settingBar = CreateBar(_arrowMenuBarAsset, arrowMenuSettingData.Label, parentSection);
            var arrowMenu = settingBar.Q<ArrowMenuString>();

            arrowMenu.Options = arrowMenuSettingData.Options;

            BindArrowMenu(arrowMenu, controlViewModel);
            BindBar(settingBar, arrowMenu, controlViewModel, arrowMenuSettingData);
        }

        private void BindArrowMenu<T>(ArrowMenu<T> arrowMenu, IControlViewModel<T> controlViewModel)
        {
            arrowMenu.OnValueChanged += controlViewModel.SetValue;
            controlViewModel.OnChanged.Subscribe(x => arrowMenu.Value = x)
                .AddTo(_disposables);
        }

        private VisualElement CreateBar(VisualTreeAsset asset, string labelText, VisualElement parent)
        {
            var settingBar = asset.CloneTree();
            parent.Add(settingBar);

            var label = settingBar.Q<Label>(name: _settingBarLabelName);
            label.text = labelText;

            return settingBar;
        }

        private void BindBar<T>(VisualElement bar, VisualElement control, IControlViewModel<T> controlViewModel,
            ISettingBarData data)
        {
            var barInfoContainer = new VisualElement
            {
                style =
                {
                    display = DisplayStyle.None
                }
            };
            barInfoContainer.AddToClassList(_infoEntryClass);
            _controlInfoContainer.Add(barInfoContainer);

            var descriptionLabel = new Label(data.Description);
            descriptionLabel.AddToClassList(_descriptionClass);
            barInfoContainer.Add(descriptionLabel);

            if (data.WarningText != null)
            {
                var warningLabel = new Label(data.WarningText);
                warningLabel.AddToClassList(_warningDescriptionClass);
                barInfoContainer.Add(warningLabel);

                controlViewModel.OnWarning.Subscribe(hasWarning =>
                    {
                        if (hasWarning)
                        {
                            warningLabel.style.display = DisplayStyle.Flex;
                            control.SetEnabled(false);
                        }
                        else
                        {
                            warningLabel.style.display = DisplayStyle.None;
                            control.SetEnabled(true);
                        }
                    })
                    .AddTo(_disposables);
            }
            else
            {
                controlViewModel.OnWarning.Subscribe(hasWarning => control.SetEnabled(!hasWarning))
                    .AddTo(_disposables);
            }

            bar.RegisterCallback<PointerEnterEvent>(_ => barInfoContainer.style.display = DisplayStyle.Flex,
                TrickleDown.TrickleDown);
            bar.RegisterCallback<PointerLeaveEvent>(_ => barInfoContainer.style.display = DisplayStyle.None,
                TrickleDown.TrickleDown);
        }

        public void Dispose()
        {
            Disposes.ClearDispose(ref _disposables);
        }
    }
}