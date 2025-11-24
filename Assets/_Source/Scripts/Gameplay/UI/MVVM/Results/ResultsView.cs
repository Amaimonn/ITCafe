using System;
using System.Collections.Generic;
using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using ITCafe.CafeBusiness;
using R3;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class ResultsView : ScreenToolkitAttach<ResultsViewModel>
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

        protected override void OnInit()
        {
            _menuButton = Root.Q<Button>(name: _menuButtonName);
            _restartButton = Root.Q<Button>(name: _restartButtonName);
            _continueButton = Root.Q<Button>(name: _continueButtonName);
            
            _billCodeLabel =  Root.Q<Label>(name: _billCodeName);
            _ordersTakenLabel = Root.Q<Label>(name: _ordersTakenName);
            _ordersCompletedLabel = Root.Q<Label>(name: _ordersCompletedName);
            _ordersFailedLabel = Root.Q<Label>(name: _ordersFailedName);
            _timeWorkedValue = Root.Q<Label>(name: _timeWorkedName);
            _pointsLabel =  Root.Q<Label>(name: _pointsName);
            
            InitStars();
        }

        protected override void OnBind(ResultsViewModel viewModel)
        {
            base.OnBind(viewModel);
            
            _menuButton.SubscribeCallback<ClickEvent>(_ => viewModel.ExitToMainMenu());
            _restartButton.SubscribeCallback<ClickEvent>(_ => viewModel.Restart());
            _continueButton.SubscribeCallback<ClickEvent>(_ => viewModel.GoNextDay());

            viewModel.OnProgressReported.Take(1).Subscribe(FillUIWithData);
        }

        private void InitStars()
        {
            _stars.Clear();

            for (var i = 1; i <= 5; i++)
            {
                var star = Root.Q<VisualElement>($"Star{i}");
                if (star != null)
                    _stars.Add(star);
            }
        }

        private void FillUIWithData(ProgressReport report)
        {
            _billCodeLabel.text = BillCodeGenerator.GetCode();
            _ordersTakenLabel.text = report.ClientsCount.ToString();
            _ordersCompletedLabel.text = report.SuccessfulOrders.ToString();
            _ordersFailedLabel.text = report.FailedOrders.ToString();
            _pointsLabel.text = report.Points.ToString();

            _timeWorkedValue.text = FormatTimeWorked(report.DayStartTime);

            UpdateStarsFromReport(report);
        }

        private string FormatTimeWorked(DateTime dayStartTime)
        {
            var timeWorked = DateTime.Now - dayStartTime;
            return $"{(int)timeWorked.TotalMinutes:00}:{timeWorked.Seconds:00}";
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
    }
}