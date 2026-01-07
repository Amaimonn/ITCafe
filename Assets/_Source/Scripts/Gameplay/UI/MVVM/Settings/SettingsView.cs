using System.Collections.Generic;
using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using Inui.UI.MVVM.Settings;
using ITCafe.Data.Settings;
using ITCafe.Gameplay.UI.Custom;
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
        [SerializeField] private string _sectionsContainerName = "SectionsContainer";
        [SerializeField] private string _tabButtonsContainerName = "TabButtonsContainer";

        [Space(4)]
        [SerializeField] private SettingControlFactory _controlFactory;

        private Button _applyButton;
        private Button _cancelChangesButton;
        private Button _closeButton;
        private VisualElement _sectionsContainer;
        private VisualElement _tabButtonsContainer;
        private readonly Dictionary<SettingsSectionViewModel, TabEntry> _sectionsMap = new();

        protected override void OnInit()
        {
            _applyButton = Root.Q<Button>(name: _applyButtonName);
            _cancelChangesButton = Root.Q<Button>(name: _cancelChangesButtonName);
            _closeButton = Root.Q<Button>(name: _closeButtonName);

            _sectionsContainer = Root.Q<VisualElement>(name: _sectionsContainerName);
            _sectionsContainer.Clear();
            _tabButtonsContainer = Root.Q<VisualElement>(name: _tabButtonsContainerName);
            _tabButtonsContainer.Clear();
        }

        protected override void OnBind(SettingsViewModel viewModel)
        {
            base.OnBind(viewModel);
            
            _sectionsMap.Clear();

            viewModel.OnSettingsDataChanged.Where(x => x != null)
                .Take(1)
                .Subscribe(data =>
                {
                    _sectionsContainer.Clear();
                    InitVideo(data);
                    InitSound(data);
                    ViewModel.OnSectionChanged.Subscribe(OnSectionChanged)
                        .AddTo(_disposables);
                })
                .AddTo(_disposables);

            _applyButton.RegisterCallback<ClickEvent>(ApplyChanges);
            _cancelChangesButton.RegisterCallback<ClickEvent>(CancelChanges);
            _closeButton.SubscribeCallbackOnce<ClickEvent>(OnCloseClicked)
                .AddTo(_disposables);

            ViewModel.IsAnyChanges.Subscribe(x =>
            {
                _applyButton.SetEnabled(x);
                _cancelChangesButton.SetEnabled(x);
            }).AddTo(_disposables);
        }

        private void InitVideo(ISettingsData data)
        {
            var videoViewModel = ViewModel.VideoSettingsViewModel;
            
            var tabEntry = _controlFactory.AddBindedTab(_sectionsContainer, _tabButtonsContainer,
                data.VideoSectionLabel, _ => ViewModel.SelectSection(videoViewModel));
            var videoSection = tabEntry.ScrollView;
            _sectionsMap[videoViewModel] = tabEntry;

            _controlFactory.AddBindedSliderInt(data.SensitivityData, videoSection,
                videoViewModel.SetSensitivity, videoViewModel.Sensitivity);

            _controlFactory.AddBindedToggle(data.VSyncData, videoSection,
                videoViewModel.SetVsync, videoViewModel.VSync);

            _controlFactory.AddBindedDropdown(data.FpsData, videoSection,
                videoViewModel.SetFps, videoViewModel.FPS);

            _controlFactory.AddBindedSliderInt(data.BrightnessData, videoSection,
                videoViewModel.SetBrightness, videoViewModel.Brightness);

            _controlFactory.AddBindedToggle(data.IsPostProcessingEnabledData, videoSection,
                videoViewModel.SetPostProcessing, videoViewModel.PostProcessing);

            _controlFactory.AddBindedToggle(data.IsBloomEnabledData, videoSection,
                videoViewModel.SetBloom, videoViewModel.Bloom);

            _controlFactory.AddBindedToggle(data.IsFilmGrainEnabledData, videoSection,
                videoViewModel.SetFilmGrain, videoViewModel.FilmGrain);

            _controlFactory.AddBindedToggle(data.IsAntiAliasingEnabledData, videoSection,
                videoViewModel.SetAntiAliasing, videoViewModel.AntiAliasing);

            _controlFactory.AddBindedDropdown(data.ResolutionData, videoSection,
                videoViewModel.SetResolution, videoViewModel.Resolution);

            _controlFactory.AddBindedToggle(data.FullscreenData, videoSection,
                videoViewModel.SetFullscreen, videoViewModel.Fullscreen);
        }

        private void InitSound(ISettingsData data)
        {
            var soundViewModel = ViewModel.SoundSettingsViewModel;
            
            var tabEntry =  _controlFactory.AddBindedTab(_sectionsContainer, _tabButtonsContainer, 
                data.SoundSectionLabel, _ => ViewModel.SelectSection(soundViewModel));
            var soundSection = tabEntry.ScrollView;
            _sectionsMap[soundViewModel] = tabEntry;
            
            _controlFactory.AddBindedSliderInt(data.MusicVolumeData, soundSection,
                soundViewModel.SetMusicVolume, soundViewModel.MusicVolume);

            _controlFactory.AddBindedSliderInt(data.SfxVolumeData, soundSection,
                soundViewModel.SetSfxVolume, soundViewModel.SfxVolume);
        }

        private void OnSectionChanged(SettingsSectionViewModel section)
        {
            foreach (var (viewModel, tabEntry) in _sectionsMap)
            {
                if (viewModel == section)
                {
                    tabEntry.Button.AddToClassList(USSConst.SELECTED);
                    tabEntry.ScrollView.style.display = DisplayStyle.Flex;
                }
                else
                {
                    tabEntry.Button.RemoveFromClassList(USSConst.SELECTED);
                    tabEntry.ScrollView.style.display = DisplayStyle.None;
                }
            }
        }

        private void ApplyChanges(ClickEvent _)
        {
            ViewModel.ApplyChanges();
        }

        private void CancelChanges(ClickEvent _)
        {
            ViewModel.CancelUnappliedChanges();
        }

        private void OnCloseClicked(ClickEvent _)
        {
            ViewModel.StartClosing();
        }
    }
}