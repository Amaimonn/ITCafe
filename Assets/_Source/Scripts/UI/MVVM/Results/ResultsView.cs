using System;
using System.Collections.Generic;
using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using ITCafe.CafeBusiness;
using ITCafe.Shared;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace ITCafe.UI.MVVM
{
    public class ResultsView : AttachableToolkitScreen<ResultsViewModel>
    {
        [SerializeField] private string _menuButtonName = "MenuButton";
        [SerializeField] private string _restartButtonName = "RestartButton";
        [SerializeField] private string _continueButtonName = "ContinueButton";

        [SerializeField] private string _ordersTakenName = "OrdersTakenValue";
        [SerializeField] private string _ordersCompletedName = "OrdersCompletedValue";
        [SerializeField] private string _ordersFailedName = "OrdersFailedValue";
        [SerializeField] private string _timeWorkedName = "TimeWorkedValue";
        [SerializeField] private string _pointsName = "PointsValue";
        [SerializeField] private string _billCodeName = "BillCodeLabel";
        
        [Header("SFX"), Space(4)]
        [SerializeField] private SfxData _buttonClickSfx;

        private Button _menuButton;
        private Button _restartButton;
        private Button _continueButton;

        private Label _billCodeLabel;
        private Label _ordersTakenLabel;
        private Label _ordersCompletedLabel;
        private Label _ordersFailedLabel;
        private Label _pointsLabel;
        private Label _timeWorkedValue;

        private readonly List<VisualElement> _stars = new();
        private readonly List<Label> _starBoundaries = new();
        
        [Inject] private readonly AudioPlayer _audioPlayer;

        protected override void OnInit()
        {
            _menuButton = Root.Q<Button>(name: _menuButtonName);
            _restartButton = Root.Q<Button>(name: _restartButtonName);
            _continueButton = Root.Q<Button>(name: _continueButtonName);

            _billCodeLabel = Root.Q<Label>(name: _billCodeName);
            _ordersTakenLabel = Root.Q<Label>(name: _ordersTakenName);
            _ordersCompletedLabel = Root.Q<Label>(name: _ordersCompletedName);
            _ordersFailedLabel = Root.Q<Label>(name: _ordersFailedName);
            _timeWorkedValue = Root.Q<Label>(name: _timeWorkedName);
            _pointsLabel = Root.Q<Label>(name: _pointsName);

            InitStars();
        }

        protected override void OnBind(ResultsViewModel viewModel)
        {
            base.OnBind(viewModel);

            _menuButton.SubscribeCallback<ClickEvent>(OnExitToMainMenuClicked);
            _restartButton.SubscribeCallback<ClickEvent>(OnRestartClicked);
            _continueButton.SubscribeCallback<ClickEvent>(OnGoNextMissionClicked);

            viewModel.OnProgressReported.Take(1).Subscribe(FillUIWithData);
        }

        private void OnExitToMainMenuClicked(ClickEvent _)
        {
            PlayButtonSfx();
            ViewModel.ExitToMainMenu();
        }

        private void OnRestartClicked(ClickEvent _)
        {
            PlayButtonSfx();
            ViewModel.Restart();
        }

        private void OnGoNextMissionClicked(ClickEvent _)
        {
            PlayButtonSfx();
            ViewModel.GoNextMission();
        }

        private void InitStars()
        {
            _stars.Clear();
            _starBoundaries.Clear();

            for (var i = 1; i <= 5; i++)
            {
                var star = Root.Q<VisualElement>($"Star{i}");
                if (star != null)
                    _stars.Add(star);

                var starBoundary = Root.Q<Label>($"StarBoundary{i}");
                if (starBoundary != null)
                    _starBoundaries.Add(starBoundary);
            }
        }

        private void FillUIWithData(ProgressReport report)
        {
            _billCodeLabel.text = BillCodeGenerator.GetCode();
            _ordersTakenLabel.text = report.ClientsCount.ToString();
            _ordersCompletedLabel.text = report.SuccessfulOrders.ToString();
            _ordersFailedLabel.text = report.FailedOrders.ToString();
            _pointsLabel.text = report.Score.ToString();

            _timeWorkedValue.text = report.WorkTime.ToString(@"mm\:ss");

            UpdateStarsFromReport(report);
            UpdateStarBoundariesFromReport(report);
        }

        private void UpdateStarsFromReport(ProgressReport report)
        {
            var earnedStars = report.EarnedStars;

            earnedStars = Math.Clamp(earnedStars, 0, _stars.Count);

            for (var i = 0; i < _stars.Count; i++)
            {
                var isFilled = i < earnedStars;
                _stars[i].EnableInClassList("results__star--filled", isFilled);
                _stars[i].EnableInClassList("results__star--empty", !isFilled);
            }
        }

        private void UpdateStarBoundariesFromReport(ProgressReport report)
        {
            var starEvaluations = report.StarEvaluations;
            int boundaryCount;
            
            if (starEvaluations.Count != _starBoundaries.Count)
            {
                boundaryCount = _starBoundaries.Count;
                FLogger.LogWarning<ResultsView>("Star boundary count mismatch");
            }
            else
            {
                boundaryCount = Mathf.Min(_starBoundaries.Count, starEvaluations.Count);
            }

            for (var i = 0; i < boundaryCount; i++)
                _starBoundaries[i].text = starEvaluations[i].ToString();
        }
        
        private void PlayButtonSfx()
        {
            if (_buttonClickSfx.IsValid)
                _audioPlayer.GetSfxBuilder().Play(_buttonClickSfx);
        }
    }
}