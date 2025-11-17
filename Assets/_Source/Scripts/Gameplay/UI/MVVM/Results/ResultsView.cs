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
        [SerializeField] private string _totalIncomeName = "TotalIncomeValue";
        [SerializeField] private string _tipsName = "TipsValue";
        [SerializeField] private string _totalName = "TotalValue";
        [SerializeField] private string _timeWorkedName = "TimeWorkedValue";
        [SerializeField] private string _speedBonusName = "SpeedBonusValue";
        [SerializeField] private string _cafeRatingName = "CafeRatingValue";

        private Button _menuButton;
        private Button _restartButton;
        private Button _continueButton;

        private Label _ordersTakenValue;
        private Label _ordersCompletedValue;
        private Label _ordersFailedValue;
        private Label _totalIncomeValue;
        private Label _tipsValue;
        private Label _totalValue;

        private Label _timeWorkedValue;
        private Label _speedBonusValue;
        private Label _cafeRatingValue;

        private readonly List<VisualElement> _stars = new();

        protected override void OnInit()
        {
            _menuButton = Root.Q<Button>(name: _menuButtonName);
            _restartButton = Root.Q<Button>(name: _restartButtonName);
            _continueButton = Root.Q<Button>(name: _continueButtonName);

            _ordersTakenValue = Root.Q<Label>(name: _ordersTakenName);
            _ordersCompletedValue = Root.Q<Label>(name: _ordersCompletedName);
            _ordersFailedValue = Root.Q<Label>(name: _ordersFailedName);
            _totalIncomeValue = Root.Q<Label>(name: _totalIncomeName);
            _tipsValue = Root.Q<Label>(name: _tipsName);
            _totalValue = Root.Q<Label>(name: _totalName);

            _timeWorkedValue = Root.Q<Label>(name: _timeWorkedName);
            _speedBonusValue = Root.Q<Label>(name: _speedBonusName);
            _cafeRatingValue = Root.Q<Label>(name: _cafeRatingName);

            InitializeStars();
        }

        protected override void OnBind(ResultsViewModel viewModel)
        {
            base.OnBind(viewModel);
            
            _menuButton.SubscribeCallback<ClickEvent>(_ => viewModel.ExitToMainMenu());
            _restartButton.SubscribeCallback<ClickEvent>(_ => viewModel.Restart());
            _continueButton.SubscribeCallback<ClickEvent>(_ => viewModel.GoNextDay());

            viewModel.OnProgressReported.Take(1).Subscribe(FillUIWithData);
        }

        private void InitializeStars()
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
            _ordersTakenValue.text = report.OrdersTaken.ToString();
            _ordersCompletedValue.text = report.SuccessfulOrders.ToString();
            _ordersFailedValue.text = report.FailedOrders.ToString();

            var baseIncome = report.SuccessfulOrders * 100;
            var tips = (int)(baseIncome * 0.15f);
            var total = baseIncome + tips;

            _totalIncomeValue.text = FormatCurrency(baseIncome);
            _tipsValue.text = FormatCurrency(tips);
            _totalValue.text = FormatCurrency(total);

            _timeWorkedValue.text = FormatTimeWorked(report.DayStartTime);
            _speedBonusValue.text = $"+{CalculateSpeedBonus(report):0}";
            _cafeRatingValue.text = $"{report.SuccessRate * 5f:0.0}/5.0";

            UpdateStarsFromReport(report);
        }

        private string FormatCurrency(int amount)
        {
            return amount.ToString("N0").Replace(",", " ");
        }

        private string FormatTimeWorked(DateTime dayStartTime)
        {
            var timeWorked = DateTime.Now - dayStartTime;
            return $"{(int)timeWorked.TotalHours:00}:{timeWorked.Minutes:00}";
        }

        private int CalculateSpeedBonus(ProgressReport report)
        {
            if (report.AverageServiceTime < 60f) 
                return 50;
            
            if (report.AverageServiceTime < 120f) 
                return 25;
            
            return 0;
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