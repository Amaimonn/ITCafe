using System.Collections;
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
        [SerializeField] private string _panelWhiteBackgroundClass = "panel__white-background";
        [SerializeField] private string _loadingIndicatorName = "LoadingIndicator";

        [Header("Assets"), Space(4)]
        [SerializeField] private VisualTreeAsset _missionButton;
        [SerializeField] private string _missionButtonSelectedClass = "campaign__mission-button--selected";
        [SerializeField] private string _missionButtonCompletedClass = "campaign__mission-button--completed";
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
        private VisualElement _loadingIndicator;
        private bool _isGameplayStarted = false;
        private bool _isClosing = false;
        private readonly Dictionary<string, Button> _missionButtonsMap = new();
        private readonly Dictionary<string, VisualElement> _locationTabButtonsMap = new();
        private Button _selectedMissionButton;
        private VisualElement _selectedLocationTab;

        protected override void OnInit()
        {
            base.OnInit();

            // Находим UI элементы
            _content = Root.Q<VisualElement>(name: _contentName);
            _startButton = Root.Q<Button>(name: _startButtonName);
            _locationTabsContainer = Root.Q<VisualElement>(name: _locationTabsContainerName);
            _selectedMissionLabel = Root.Q<Label>(name: _selectedMissionLabelName);
            _selectedMissionText = Root.Q<Label>(name: _selectedMissionTextName);
            _missionTextScrollView = Root.Q<ScrollView>(name: _missionTextScrollViewName);
            _missionsGrid = Root.Q<VisualElement>(name: _missionsGridName);
            _panelWhiteBackground = Root.Q<VisualElement>(className: _panelWhiteBackgroundClass);
            _loadingIndicator = Root.Q<VisualElement>(name: _loadingIndicatorName);

            // Начальное состояние
            // _content.AddToClassList($"{_contentClass}--disabled");
            // _content.RegisterCallback<TransitionEndEvent>(_ =>
            // {
            //     if (_isClosing)
            //         ViewModel.CompleteClosing();
            // });

            if (_loadingIndicator != null)
                _loadingIndicator.style.display = DisplayStyle.None;
        }

        protected override void OnBind(CampaignViewModel viewModel)
        {
            base.OnBind(viewModel);

            ViewModel.LocationsDataMap.Subscribe(OnLocationsChanged).AddTo(_disposables);
            ViewModel.SelectedLocationData.Subscribe(OnLocationSelected).AddTo(_disposables);
            ViewModel.SelectedMissionData.Subscribe(OnMissionSelected).AddTo(_disposables);
            ViewModel.CurrentMissionsData.Subscribe(OnCurrentMissionsChanged).AddTo(_disposables);

            _startButton.RegisterCallback<ClickEvent>(StartGameplay);
        }

        private void OnLocationsChanged(IReadOnlyDictionary<string, ILocationData> locations)
        {
            if (_locationTabsContainer == null || locations == null)
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

                // Проверяем доступность локации
                if (ViewModel.OpenedLocationsMap.TryGetValue(locationData.Id, out var locationModel))
                {
                    locationTabButton.RegisterCallback<ClickEvent>(_ => ViewModel.SelectLocation(locationData));
                    _locationTabButtonsMap[locationData.Id] = locationTabButton;

                    // Подсветка выбранной локации
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
            }
        }

        private void OnCurrentMissionsChanged(IReadOnlyList<IMissionData> missions)
        {
            if (missions == null)
            {
                FLogger.Log<CampaignView>($"Current missions data is null");
                return;
            }
            
            FLogger.Log<CampaignView>($"Current missions data: {missions.Count}");

            ShowLoadingIndicator(false);
            _missionsGrid.Clear();
            _missionButtonsMap.Clear();

            if (missions.Count == 0)
            {
                // Можно добавить сообщение о том, что миссии загружаются
                Debug.Log("No missions available for this location");
                return;
            }

            var selectedMissionId = ViewModel.SelectedMissionData.CurrentValue?.Id;

            foreach (var missionData in missions)
            {
                var missionButtonContainer = _missionButton.CloneTree();
                var missionButton = missionButtonContainer.Q<Button>();
                var missionLabel = missionButtonContainer.Q<Label>();

                missionLabel.text = missionData.DisplayedNumber;

                // Проверяем доступность миссии
                if (ViewModel.OpenedMissionsMap.TryGetValue(missionData.Id, out var missionModel))
                {
                    // Отмечаем выполненную миссию
                    if (missionModel.IsCompleted.Value)
                    {
                        missionButton.AddToClassList(_missionButtonCompletedClass);
                        var starsColumn = missionButtonContainer.Q<VisualElement>(name: _starsColumnName);
                        if (starsColumn != null && _starLi != null)
                        {
                            for (var i = 0; i < missionModel.Stars; i++)
                                _starLi.CloneTree(starsColumn);
                        }
                    }

                    missionButton.RegisterCallback<ClickEvent>(_ => ViewModel.SelectMission(missionData));
                    _missionButtonsMap[missionData.Id] = missionButton;

                    // Подсветка выбранной миссии
                    if (!string.IsNullOrEmpty(selectedMissionId) && missionData.Id == selectedMissionId)
                    {
                        missionButton.AddToClassList(_missionButtonSelectedClass);
                        _selectedMissionButton = missionButton;
                        UpdateMissionDetails(missionData);
                    }
                }
                else
                {
                    missionButton.SetEnabled(false);
                }

                _missionsGrid.Add(missionButtonContainer);
            }
        }

        private void OnLocationSelected(ILocationData locationData)
        {
            if (locationData == null)
                return;

            // Показываем индикатор загрузки при смене локации
            ShowLoadingIndicator(true);

            // Обновляем выделение вкладки локации
            _selectedLocationTab?.RemoveFromClassList(_locationButtonSelectedClass);
            if (_locationTabButtonsMap.TryGetValue(locationData.Id, out var tabButton))
            {
                tabButton.AddToClassList(_locationButtonSelectedClass);
                _selectedLocationTab = tabButton;
            }

            // Очищаем информацию о выбранной миссии до загрузки новых
            _selectedMissionButton = null;
            _selectedMissionLabel.text = string.Empty;
            _selectedMissionText.text = string.Empty;
            _startButton.SetEnabled(false);
        }

        private void OnMissionSelected(IMissionData missionData)
        {
            if (missionData == null)
            {
                _startButton.SetEnabled(false);
                _selectedMissionLabel.text = string.Empty;
                _selectedMissionText.text = string.Empty;
                return;
            }

            // Обновляем выделение кнопки миссии
            _selectedMissionButton?.RemoveFromClassList(_missionButtonSelectedClass);

            // Проверяем доступность миссии для кнопки старта
            if (ViewModel.OpenedMissionsMap.ContainsKey(missionData.Id))
                _startButton.SetEnabled(true);
            else
                _startButton.SetEnabled(false);

            // Находим и выделяем кнопку миссии
            if (_missionButtonsMap.TryGetValue(missionData.Id, out var button))
            {
                button.AddToClassList(_missionButtonSelectedClass);
                _selectedMissionButton = button;
                UpdateMissionDetails(missionData);
            }
            else
            {
                Debug.LogWarning($"No button found for missionId: {missionData.Id}");
            }

            _missionTextScrollView.scrollOffset = Vector2.zero;
        }

        private void UpdateMissionDetails(IMissionData missionData)
        {
            if (_selectedMissionLabel != null)
                _selectedMissionLabel.text = missionData.Name;

            if (_selectedMissionText != null)
                _selectedMissionText.text = missionData.Description;
        }

        private void StartGameplay(ClickEvent clickEvent)
        {
            if (_isGameplayStarted)
                return;

            ViewModel.StartGameplay();
            _isGameplayStarted = true;
        }

        private void ShowLoadingIndicator(bool show)
        {
            if (_loadingIndicator != null)
                _loadingIndicator.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;

            if (_missionsGrid != null)
                _missionsGrid.style.display = show ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
}