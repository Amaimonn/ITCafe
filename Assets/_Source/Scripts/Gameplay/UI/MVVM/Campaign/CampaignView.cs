using System.Collections.Generic;
using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using UnityEngine;
using UnityEngine.UIElements;
using R3;
using ITCafe.Data.Campaign;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class CampaignView : AttachableToolkitWindow<CampaignViewModel>
    {
        [Header("UI Elements")]
        [SerializeField] private string _contentName = "RootWrapper";
        [SerializeField] private string _contentClass = "campaign__root";
        [SerializeField] private string _startButtonName = "StartButton";
        [SerializeField] private string _locationTabsContainerName = "LocationsTabsContainer";
        [SerializeField] private string _selectedMissionLabelName = "SelectedMissionLabel";
        [SerializeField] private string _selectedMissionTextName = "SelectedMissionText";
        [SerializeField] private string _missionTextScrollViewName = "MissionTextScrollView";
        [SerializeField] private string _missionsGridName = "MissionsGrid";

        [Header("Assets"), Space(4)]
        [SerializeField] private VisualTreeAsset _missionButton;
        [SerializeField] private string _missionButtonSelectedClass = "campaign__mission-button--selected";
        [SerializeField] private string _missionButtonCompletedClass = "campaign__mission-button--completed";
        [SerializeField] private string _missionButtonLockedClass = "campaign__mission-button--locked";

        [SerializeField] private VisualTreeAsset _starLi;
        [SerializeField] private string _starsColumnName = "StarsColumn";

        [Space(2f)]
        [SerializeField] private VisualTreeAsset _locationTabButton;
        [SerializeField] private string _locationButtonSelectedClass = "campaign__location-tab--selected";

        private Button _startButton;
        private VisualElement _content;
        private VisualElement _locationTabsContainer;
        private Label _selectedMissionLabel;
        private Label _selectedMissionText;
        private ScrollView _missionTextScrollView;
        private VisualElement _missionsGrid;
        private VisualElement _panelWhiteBackground;
        private bool _isGameplayStarted = false;
        private bool _isClosing = false;
        private readonly Dictionary<string, Button> _missionButtonsMap = new();
        private readonly Dictionary<string, VisualElement> _locationTabButtonsMap = new();
        private Button _selectedMissionButton;
        private VisualElement _selectedLocationTab;

        protected override void OnInit()
        {
            base.OnInit();

            _content = Root.Q<VisualElement>(name: _contentName);
            _startButton = Root.Q<Button>(name: _startButtonName);
            _locationTabsContainer = Root.Q<VisualElement>(name: _locationTabsContainerName);
            _selectedMissionLabel = Root.Q<Label>(name: _selectedMissionLabelName);
            _selectedMissionText = Root.Q<Label>(name: _selectedMissionTextName);
            _missionTextScrollView = Root.Q<ScrollView>(name: _missionTextScrollViewName);
            _missionsGrid = Root.Q<VisualElement>(name: _missionsGridName);

            // Начальное состояние
            // _content.AddToClassList($"{_contentClass}--disabled");
            // _content.RegisterCallback<TransitionEndEvent>(_ =>
            // {
            //     if (_isClosing)
            //         ViewModel.CompleteClosing();
            // });
        }

        protected override void OnBind(CampaignViewModel viewModel)
        {
            base.OnBind(viewModel);

            ViewModel.LocationsDataMap.Subscribe(OnLocationsChanged).AddTo(_disposables);
            ViewModel.SelectedLocationData.Subscribe(OnLocationSelected).AddTo(_disposables);
            ViewModel.CurrentMissionsData.Subscribe(OnCurrentMissionsChanged).AddTo(_disposables);
            ViewModel.SelectedMissionData.Subscribe(OnMissionSelected).AddTo(_disposables);

            _startButton.RegisterCallback<ClickEvent>(StartGameplay);
        }

        private void OnLocationsChanged(IReadOnlyDictionary<string, ILocationData> locations)
        {
            if (locations == null)
                return;

            _locationTabsContainer.Clear();
            _locationTabButtonsMap.Clear();

            var locationsData = locations.Values;
            var selectedLocationId = ViewModel.SelectedLocationData.CurrentValue?.Id;

            foreach (var locationData in locationsData)
            {
                var locationTabButtonContainer = _locationTabButton.CloneTree();
                var locationTabButton = locationTabButtonContainer.Q<Button>();
                var locationLabel = locationTabButtonContainer.Q<Label>();

                locationLabel.text = locationData.Name;

                if (ViewModel.OpenedLocationsMap.TryGetValue(locationData.Id, out _))
                {
                    locationTabButton.RegisterCallback<ClickEvent>(_ => ViewModel.SelectLocation(locationData));
                    _locationTabButtonsMap[locationData.Id] = locationTabButton;

                    if (!string.IsNullOrEmpty(selectedLocationId) && locationData.Id == selectedLocationId)
                    {
                        locationTabButton.AddToClassList(_locationButtonSelectedClass);
                        _selectedLocationTab = locationTabButton;
                    }
                }
                else
                {
                    FLogger.Log<CampaignView>($"No missions opened for: {locationData.Id}");
                    locationTabButton.SetEnabled(false);
                }

                _locationTabsContainer.Add(locationTabButtonContainer);

                var selectedLocationData = ViewModel.SelectedLocationData.CurrentValue;
                if (selectedLocationData != null && locations.ContainsKey(selectedLocationData.Id))
                    OnLocationSelected(selectedLocationData);
            }
        }

        private void OnCurrentMissionsChanged(IReadOnlyList<IMissionData> missions)
        {
            _missionsGrid.Clear();
            _missionButtonsMap.Clear();

            if (missions == null || missions.Count == 0)
            {
                FLogger.Log<CampaignView>($"No Current missions data");
                return;
            }

            FLogger.Log<CampaignView>($"Current missions data: {missions.Count}");

            foreach (var missionData in missions)
            {
                var missionButtonContainer = _missionButton.CloneTree();
                var missionButton = missionButtonContainer.Q<Button>();
                var missionLabel = missionButtonContainer.Q<Label>();

                missionLabel.text = missionData.DisplayedNumber;

                _missionButtonsMap[missionData.Id] = missionButton;
                missionButton.RegisterCallback<ClickEvent>(_ => ViewModel.SelectMission(missionData));

                if (ViewModel.OpenedMissionsMap.TryGetValue(missionData.Id, out var missionModel))
                {
                    if (missionModel.IsCompleted.Value)
                    {
                        missionButton.AddToClassList(_missionButtonCompletedClass);
                        var starsColumn = missionButtonContainer.Q<VisualElement>(name: _starsColumnName);

                        for (var i = 0; i < missionModel.Stars.Value; i++)
                            _starLi.CloneTree(starsColumn);
                    }
                }
                else
                {
                    missionButtonContainer.AddToClassList(_missionButtonLockedClass);
                }

                if (ViewModel.SelectedMissionData.CurrentValue != null &&
                    ViewModel.SelectedMissionData.CurrentValue.Id == missionData.Id)
                {
                    OnMissionSelected(missionData);
                }

                _missionsGrid.Add(missionButtonContainer);
            }
        }

        private void OnLocationSelected(ILocationData locationData)
        {
            if (locationData == null)
            {
                FLogger.Log<CampaignView>($"No location data");
                _selectedLocationTab?.RemoveFromClassList(_locationButtonSelectedClass);
                _selectedLocationTab = null;
                return;
            }

            _selectedLocationTab?.RemoveFromClassList(_locationButtonSelectedClass);
            if (_locationTabButtonsMap.TryGetValue(locationData.Id, out var tabButton))
            {
                tabButton.AddToClassList(_locationButtonSelectedClass);
                _selectedLocationTab = tabButton;
            }
            else
            {
                FLogger.LogWarning<CampaignView>($"No tab found for location: {locationData.Id}");
                _selectedLocationTab = null;
            }
        }

        private void OnMissionSelected(IMissionData missionData)
        {
            _selectedMissionButton?.RemoveFromClassList(_missionButtonSelectedClass);

            if (missionData != null)
            {
                var isOpened = ViewModel.OpenedMissionsMap.ContainsKey(missionData.Id);
                _startButton.SetEnabled(isOpened);
                _selectedMissionLabel.text = missionData.Name;
                _selectedMissionText.text = missionData.Description;

                if (_missionButtonsMap.TryGetValue(missionData.Id, out var button))
                {
                    button.AddToClassList(_missionButtonSelectedClass);
                    _selectedMissionButton = button;
                }
                else
                {
                    _selectedMissionButton = null;
                    Debug.LogWarning($"No button found for missionId: {missionData.Id}");
                }

                _missionTextScrollView.scrollOffset = Vector2.zero;
            }
            else
            {
                _startButton.SetEnabled(false);
                _selectedMissionButton = null;
                _selectedMissionLabel.text = string.Empty;
                _selectedMissionText.text = string.Empty;
            }
        }

        private void StartGameplay(ClickEvent clickEvent)
        {
            if (_isGameplayStarted)
                return;

            ViewModel.StartGameplay();
            _isGameplayStarted = true;
        }
    }
}