using System.Collections;
using System.Collections.Generic;
using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using UnityEngine;
using UnityEngine.UIElements;
using R3;
using ITCafe.Data.Campaign;
using ITCafe.Gameplay.UI.Custom;
using UnityEngine.UIElements.Experimental;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class CampaignView : AttachableToolkitWindow<CampaignViewModel>
    {
        [Header("UI Elements")]
        [SerializeField] private string _panelBackgroundName = "CampaignBackground";
        [SerializeField] private string _startButtonName = "StartButton";
        [SerializeField] private string _locationTabsContainerName = "LocationsTabsContainer";
        [SerializeField] private string _missionPanelName = "MissionInfoSection";
        [SerializeField] private string _selectedMissionLabelName = "SelectedMissionLabel";
        [SerializeField] private string _selectedMissionTextName = "SelectedMissionText";
        [SerializeField] private string _missionTextScrollViewName = "MissionTextScrollView";
        [SerializeField] private string _missionsContainerName = "MissionsContainer";
        [SerializeField] private string _campaignMapName = "CampaignMap";
        [SerializeField] private string _targetSelectionName = "TargetSelection";

        [Header("Assets"), Space(4)]
        [SerializeField] private VisualTreeAsset _missionButton;
        [SerializeField] private string _missionName = "MissionName";

        [Space(2f)]
        [SerializeField] private VisualTreeAsset _starLi;
        [SerializeField] private string _starsColumnName = "StarsColumn";

        [Space(2f)]
        [SerializeField] private VisualTreeAsset _locationTabButton;

        private Button _startButton;
        private VisualElement _missionPanel;
        private VisualElement _panelBackground;
        private VisualElement _campaignMap;
        private VisualElement _locationTabsContainer;
        private Label _selectedMissionLabel;
        private Label _selectedMissionText;
        private ScrollView _missionTextScrollView;
        private VisualElement _missionsContainer;
        private VisualElement _panelWhiteBackground;
        private NodesElement _nodes;
        private VisualElement _missionAim;

        private Button _selectedMissionButton;
        private VisualElement _selectedLocationTab;
        private bool _isClosing = false;
        private readonly Dictionary<string, Button> _missionButtonsMap = new();
        private readonly Dictionary<string, VisualElement> _locationTabButtonsMap = new();
        private Coroutine _showMapCoroutine;
        private Coroutine _selectionCoroutine;
        private IValueAnimation _selectionAnimation;

        protected override void OnInit()
        {
            base.OnInit();

            _panelBackground = Root.Q<VisualElement>(name: _panelBackgroundName);
            _missionPanel = Root.Q<VisualElement>(name: _missionPanelName);
            SetMissionPanelVisible(false);
            _campaignMap = Root.Q<VisualElement>(name: _campaignMapName);
            _startButton = Root.Q<Button>(name: _startButtonName);
            _locationTabsContainer = Root.Q<VisualElement>(name: _locationTabsContainerName);
            _locationTabsContainer.Clear();
            _selectedMissionLabel = Root.Q<Label>(name: _selectedMissionLabelName);
            _selectedMissionText = Root.Q<Label>(name: _selectedMissionTextName);
            _missionTextScrollView = Root.Q<ScrollView>(name: _missionTextScrollViewName);
            _missionsContainer = Root.Q<VisualElement>(name: _missionsContainerName);
            _missionsContainer.Clear();
            _nodes = Root.Q<NodesElement>();
            _nodes.ClearConnections();
            _missionAim = Root.Q<VisualElement>(name: _targetSelectionName);
            HideMissionAim();
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
            ViewModel.CurrentMissionsDataMap.Subscribe(OnCurrentMissionsChanged).AddTo(_disposables);
            ViewModel.SelectedMissionData.Subscribe(OnMissionSelected).AddTo(_disposables);

            _startButton.SubscribeCallbackOnce<ClickEvent>(StartGameplay).AddTo(_disposables);

            _panelBackground.RegisterCallback<ClickEvent>(_ => ViewModel.SelectMission(null));
        }

        protected override void OnOpening()
        {
            Show();
            SetCampaignMapEnabled(false);
            StartCoroutine(OpenAnimation());

            IEnumerator OpenAnimation()
            {
                yield return null;
                SetCampaignMapEnabled(true);
            }
        }

        private void StartGameplay(ClickEvent _)
        {
            ViewModel.StartGameplay();
        }

        private void SelectLocation(ClickEvent _, ILocationData locationData)
        {
            ViewModel.SelectLocation(locationData);
        }

        private void SelectMission(ClickEvent e, IMissionData missionData)
        {
            ViewModel.SelectMission(missionData);
            e.StopPropagation();
        }

        private void OnLocationsChanged(IReadOnlyDictionary<string, ILocationData> locationDataMap)
        {
            if (locationDataMap == null)
                return;

            _locationTabsContainer.Clear();
            _locationTabButtonsMap.Clear();

            foreach (var locationData in locationDataMap.Values)
                AddLocationTab(locationData);

            var selectedLocationData = ViewModel.SelectedLocationData.CurrentValue;
            if (selectedLocationData != null && locationDataMap.ContainsKey(selectedLocationData.Id))
                OnLocationSelected(selectedLocationData);
        }

        private void AddLocationTab(ILocationData locationData)
        {
            var locationTabButtonContainer = _locationTabButton.CloneTree();
            var locationTabButton = locationTabButtonContainer.Q<Button>();
            var locationLabel = locationTabButtonContainer.Q<Label>();

            locationLabel.text = locationData.Name;

            _locationTabButtonsMap[locationData.Id] = locationTabButton;
            locationTabButton.RegisterCallback<ClickEvent, ILocationData>(SelectLocation, locationData);

            if (ViewModel.OpenedLocationsMap.TryGetValue(locationData.Id, out var locationModel))
            {
                if (locationModel.IsCompleted.Value)
                    locationTabButton.AddToClassList(USSConst.COMPLETED);
            }
            else
            {
                FLogger.Log<CampaignView>($"No missions opened for: {locationData.Id}");
                locationTabButtonContainer.AddToClassList(USSConst.LOCKED);
            }

            _locationTabsContainer.Add(locationTabButtonContainer);
        }

        private void OnCurrentMissionsChanged(IReadOnlyDictionary<string, IMissionData> missionDataMap)
        {
            _missionsContainer.Clear();
            _missionButtonsMap.Clear();
            _nodes.ClearConnections();
            SetCampaignMapEnabled(false);

            if (missionDataMap == null || missionDataMap.Count == 0)
            {
                FLogger.Log<CampaignView>("No Current missions data");
                return;
            }

            FLogger.Log<CampaignView>($"Current missions data: {missionDataMap.Count}");

            foreach (var missionData in missionDataMap.Values)
                AddMission(missionData);

            var selectedMissionData = ViewModel.SelectedMissionData.CurrentValue;
            if (selectedMissionData != null)
                OnMissionSelected(selectedMissionData);

            DrawNodes(missionDataMap, _missionButtonsMap);

            SetCampaignMapEnabled(true);
        }

        private void AddMission(IMissionData missionData)
        {
            var missionButtonContainer = _missionButton.CloneTree();
            missionButtonContainer.pickingMode = PickingMode.Ignore;
            missionButtonContainer.style.position = Position.Absolute;
            missionButtonContainer.style.left = missionData.PositionX;
            missionButtonContainer.style.top = missionData.PositionY;

            var missionLabel = missionButtonContainer.Q<Label>();
            missionLabel.text = missionData.DisplayedNumber;

            var missionButton = missionButtonContainer.Q<Button>();
            _missionButtonsMap[missionData.Id] = missionButton;
            missionButton.RegisterCallback<ClickEvent, IMissionData>(SelectMission, missionData);
            
            var missionName = missionButtonContainer.Q<Label>(name: _missionName);
            missionName.text = missionData.Name;

            if (ViewModel.OpenedMissionsMap.TryGetValue(missionData.Id, out var missionModel))
            {
                if (missionModel.IsCompleted.Value)
                {
                    missionButton.AddToClassList(USSConst.COMPLETED);
                    var starsColumn = missionButtonContainer.Q<VisualElement>(name: _starsColumnName);

                    for (var i = 0; i < missionModel.Stars.Value; i++)
                        _starLi.CloneTree(starsColumn);
                }
            }
            else
            {
                missionButton.AddToClassList(USSConst.LOCKED);
            }

            _missionsContainer.Add(missionButtonContainer);
        }

        /// <summary>
        /// Добавить соединения на основе данных миссий
        /// </summary>
        private void DrawNodes(IReadOnlyDictionary<string, IMissionData> missions,
            Dictionary<string, Button> missionButtons)
        {
            if (missions == null)
                return;

            foreach (var mission in missions.Values)
            {
                if (mission.NextMissionIds == null ||
                    mission.NextMissionIds.Count == 0 ||
                    !missionButtons.TryGetValue(mission.Id, out var fromElement))
                {
                    continue;
                }

                foreach (var nextMissionId in mission.NextMissionIds)
                    if (missionButtons.TryGetValue(nextMissionId, out var toElement))
                        _nodes.AddConnection(fromElement, toElement);
            }
        }

        /// <summary>
        /// Mission Nodes fade animation
        /// </summary>
        private void SetCampaignMapEnabled(bool isEnabled)
        {
            if (_showMapCoroutine != null)
            {
                StopCoroutine(_showMapCoroutine);
                _showMapCoroutine = null;
            }

            if (isEnabled)
                _showMapCoroutine = StartCoroutine(ShowMap());
            else
                _campaignMap.RemoveFromClassList(USSConst.ENABLED);

            IEnumerator ShowMap()
            {
                yield return null;
                _campaignMap.AddToClassList(USSConst.ENABLED);
                _showMapCoroutine = null;
            }
        }

        private void OnLocationSelected(ILocationData locationData)
        {
            if (locationData == null)
            {
                FLogger.Log<CampaignView>("No location data");
                _selectedLocationTab?.RemoveFromClassList(USSConst.SELECTED);
                _selectedLocationTab = null;
                return;
            }

            _selectedLocationTab?.RemoveFromClassList(USSConst.SELECTED);
            if (_locationTabButtonsMap.TryGetValue(locationData.Id, out var tabButton))
            {
                tabButton.AddToClassList(USSConst.SELECTED);
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
            _selectedMissionButton?.RemoveFromClassList(USSConst.SELECTED);

            if (missionData != null)
            {
                var isOpened = ViewModel.OpenedMissionsMap.ContainsKey(missionData.Id);
                _startButton.SetEnabled(isOpened);
                _selectedMissionLabel.text = missionData.Name;
                _selectedMissionText.text = missionData.Description;
                SetMissionPanelVisible(true);

                if (_missionButtonsMap.TryGetValue(missionData.Id, out var button))
                {
                    button.AddToClassList(USSConst.SELECTED);
                    _selectedMissionButton = button;

                    AimAtMission(button);
                }
                else
                {
                    _selectedMissionButton = null;
                    HideMissionAim();
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
                HideMissionAim();
                SetMissionPanelVisible(false);
            }
        }

        private void SetMissionPanelVisible(bool isVisible)
        {
            _missionPanel.style.visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
        }

        private void AimAtMission(VisualElement target)
        {
            if (_selectionCoroutine != null)
            {
                StopCoroutine(_selectionCoroutine);
                _selectionCoroutine = null;
            }

            if (target != null)
                StartCoroutine(SelectCoroutine());
            else
                HideMissionAim();

            return;

            IEnumerator SelectCoroutine()
            {
                yield return null;
                _missionAim.style.visibility = Visibility.Visible;

                var targetCenter = _campaignMap.WorldToLocal(target.worldBound.center);
                targetCenter.x -= _missionAim.resolvedStyle.width / 2f;
                targetCenter.y -= _missionAim.resolvedStyle.height / 2f;

                _missionAim.style.translate = targetCenter;
                _selectionCoroutine = null;
            }
        }

        private void HideMissionAim()
        {
            _missionAim.style.visibility = Visibility.Hidden;
        }
    }
}