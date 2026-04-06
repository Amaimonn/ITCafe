using System.Collections.Generic;
using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using Inui.UI.MVVM.Settings;
using ITCafe.Data;
using ITCafe.Data.Settings;
using ITCafe.Shared;
using ITCafe.UI.Custom;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace ITCafe.UI.MVVM
{
    public class SettingsView : AttachableToolkitScreen<SettingsViewModel>
    {
        [Header("UIElements")]
        [SerializeField] private string _applyButtonName = "ApplyButton";
        [SerializeField] private string _cancelChangesButtonName = "CancelChangesButton";
        [SerializeField] private string _closeButtonName = "CloseButton";
        [SerializeField] private string _sectionsContainerName = "SectionsContainer";
        [SerializeField] private string _tabButtonsContainerName = "TabButtonsContainer";
        [SerializeField] private string _controlInfoContainerName = "ControlInfoContainer";

        [Space(4)]
        [SerializeField] private SettingControlFactory _controlFactory;
        
        [Header("SFX"), Space(4)]
        [SerializeField] private SfxData _buttonClickSfx;
        [SerializeField] private SfxData _resetClickSfx;
        [SerializeField] private SfxData _closeClickSfx;
        
        [Header("PopUp"), Space(4)]
        [SerializeField] private ConfirmationSetup _confirmSetup;

        private Button _applyButton;
        private Button _cancelChangesButton;
        private Button _closeButton;
        private VisualElement _sectionsContainer;
        private VisualElement _tabButtonsContainer;
        private VisualElement _controlInfoContainer;
        private readonly Dictionary<SettingsSectionViewModel, TabEntry> _sectionsMap = new();
        private CompositeDisposable _disposables;
        
        [Inject] private readonly AudioPlayer _audioPlayer;

        protected override void OnInit()
        {
            _applyButton = Root.Q<Button>(name: _applyButtonName);
            _cancelChangesButton = Root.Q<Button>(name: _cancelChangesButtonName);
            _closeButton = Root.Q<Button>(name: _closeButtonName);

            _sectionsContainer = Root.Q<VisualElement>(name: _sectionsContainerName);
            _sectionsContainer.Clear();
            _tabButtonsContainer = Root.Q<VisualElement>(name: _tabButtonsContainerName);
            _tabButtonsContainer.Clear();
            _controlInfoContainer = Root.Q<VisualElement>(name: _controlInfoContainerName);
            _controlInfoContainer.Clear();

            _controlFactory.Init(_sectionsContainer, _tabButtonsContainer, _controlInfoContainer);
        }

        protected override void OnBind(SettingsViewModel viewModel)
        {
            base.OnBind(viewModel);
            
            _disposables = new CompositeDisposable();
            _sectionsMap.Clear();
            
            viewModel.SetConfirmation(_confirmSetup);

            viewModel.OnSettingsDataChanged.Where(x => x != null)
                .Take(1)
                .Subscribe(data =>
                {
                    _sectionsContainer.Clear();
                    InitSections(data);
                    ViewModel.OnSectionChanged.Subscribe(OnSectionChanged)
                        .AddTo(_disposables);
                })
                .AddTo(_disposables);

            _applyButton.SubscribeCallback<ClickEvent>(ApplyChanges)
                .AddTo(_disposables);
            _cancelChangesButton.SubscribeCallback<ClickEvent>(CancelChanges)
                .AddTo(_disposables);
            _closeButton.SubscribeCallbackOnce<ClickEvent>(OnCloseClicked)
                .AddTo(_disposables);

            ViewModel.IsAnyChanges.Subscribe(x =>
            {
                _applyButton.SetEnabled(x);
                _cancelChangesButton.SetEnabled(x);
            }).AddTo(_disposables);

            _controlFactory.AddTo(_disposables);
        }

        private void InitSections(ISettingsData data)
        {
            InitVideo(data);
            InitSound(data);
            InitLanguage(data);
            InitInputs(data);
        }

        private void InitVideo(ISettingsData data)
        {
            var videoViewModel = ViewModel.VideoSettingsViewModel;

            var tabEntry = _controlFactory.AddBindedTab(data.VideoSectionLabel,
                GetOnTabClicked(videoViewModel), data.VideoIcon);
            var videoSection = tabEntry.ScrollView;
            _sectionsMap[videoViewModel] = tabEntry;

            _controlFactory.AddBindedToggle(data.VSyncData, videoSection, videoViewModel.VSync);

            _controlFactory.AddBindedDropdown(data.FpsData, videoSection, videoViewModel.FPS);

            _controlFactory.AddBindedSliderInt(data.BrightnessData, videoSection, videoViewModel.Brightness);

            _controlFactory.AddBindedToggle(data.IsPostProcessingEnabledData, videoSection,
                videoViewModel.PostProcessing);

            _controlFactory.AddBindedToggle(data.IsBloomEnabledData, videoSection, videoViewModel.Bloom);

            _controlFactory.AddBindedToggle(data.IsFilmGrainEnabledData, videoSection, videoViewModel.FilmGrain);

            _controlFactory.AddBindedToggle(data.IsChromaticAberrationEnabledData, videoSection,
                videoViewModel.ChromaticAberration);

            _controlFactory.AddBindedToggle(data.IsAntiAliasingEnabledData, videoSection, videoViewModel.AntiAliasing);

            _controlFactory.AddBindedDropdown(data.ResolutionData, videoSection, videoViewModel.Resolution);

            _controlFactory.AddBindedToggle(data.FullscreenData, videoSection, videoViewModel.Fullscreen);
        }

        private void InitSound(ISettingsData data)
        {
            var soundViewModel = ViewModel.SoundSettingsViewModel;

            var tabEntry = _controlFactory.AddBindedTab(data.SoundSectionLabel,
                GetOnTabClicked(soundViewModel), data.SoundIcon);
            var soundSection = tabEntry.ScrollView;
            _sectionsMap[soundViewModel] = tabEntry;

            _controlFactory.AddBindedSliderInt(data.MusicVolumeData, soundSection, soundViewModel.MusicVolume);

            _controlFactory.AddBindedSliderInt(data.SfxVolumeData, soundSection, soundViewModel.SfxVolume);
        }

        private void InitLanguage(ISettingsData data)
        {
            var languageViewModel = ViewModel.LanguageSettingsViewModel;

            var tabEntry = _controlFactory.AddBindedTab(data.LanguageSectionLabel,
                GetOnTabClicked(languageViewModel), data.LanguageIcon);
            var languageSection = tabEntry.ScrollView;
            _sectionsMap[languageViewModel] = tabEntry;

            _controlFactory.AddBindedDropdown(data.LanguageData, languageSection, languageViewModel.Language);
        }

        private void InitInputs(ISettingsData data)
        {
            var inputsViewModel = ViewModel.InputSettingsViewModel;

            var tabEntry = _controlFactory.AddBindedTab(data.InputSectionLabel,
                GetOnTabClicked(inputsViewModel), data.InputIcon);
            var inputSection = tabEntry.ScrollView;
            _sectionsMap[inputsViewModel] = tabEntry;

            _controlFactory.AddBindedSliderInt(data.SensitivityData, inputSection, inputsViewModel.Sensitivity);
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

        private EventCallback<ClickEvent> GetOnTabClicked(SettingsSectionViewModel sectionViewModel)
        {
            return _ =>
            {
                PlayButtonSfx();
                ViewModel.SelectSection(sectionViewModel);
            };
        }
        
        private void ApplyChanges(ClickEvent _)
        {
            PlayButtonSfx();
            ViewModel.ApplyChanges();
        }

        private void CancelChanges(ClickEvent _)
        {
            PlayResetSfx();
            ViewModel.CancelUnappliedChanges();
        }

        private void OnCloseClicked(ClickEvent _)
        {
            PlayCloseSfx();
            ViewModel.StartClosing();
        }
        
        private void PlayButtonSfx()
        {
            if (_buttonClickSfx.IsValid)
                _audioPlayer.GetSfxBuilder().Play(_buttonClickSfx);
        }
        
        private void PlayCloseSfx()
        {
            if (_closeClickSfx.IsValid)
                _audioPlayer.GetSfxBuilder().Play(_closeClickSfx);
        }
        
        private void PlayResetSfx()
        {
            if (_resetClickSfx.IsValid)
                _audioPlayer.GetSfxBuilder().Play(_resetClickSfx);
        }
        
        public override void Dispose()
        {
            Disposes.ClearDispose(ref _disposables);
            base.Dispose();
        }
    }
}