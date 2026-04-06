using UnityEngine;
using UnityEngine.UIElements;
using R3;
using System.Collections.Generic;
using System.Linq;
using DevKit.Locator;
using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using ITCafe.Shared;
using ObservableCollections;
using VContainer;

namespace ITCafe.UI.MVVM
{
    public class HUDView : AttachableToolkitScreen<HUDViewModel>
    {
        [SerializeField] private string _timerName = "Timer";
        [SerializeField] private string _ordersContainerName = "OrdersContainer";
        [SerializeField] private string _scoreLabelName = "ScoreValue";
        [SerializeField] private string _ordersTakenValueName = "OrderValue";
        [SerializeField] private string _ordersCompletedValueName = "SatisfactionValue";
        [SerializeField] private string _ordersFailedValueName = "FailedValue";

        [SerializeField] private VisualTreeAsset _orderAsset;
        [SerializeField] private string _foodContainerName = "FoodContainer";
        
        [SerializeField] private VisualTreeAsset _foodItemAsset;
        [SerializeField] private string _foodImageName = "FoodImage";
        [SerializeField] private string _foodLabelName = "FoodName";

        private Label _timerLabel;
        private Label _scoreLabel;
        private Label _ordersTakenLabel;
        private Label _ordersCompletedLabel;
        private Label _ordersFailedLabel;
        private VisualElement _ordersContainer;
        private ILocalizer _localizer;
        private CompositeDisposable _disposables;

        private readonly Dictionary<VisualElement, List<(int, VisualElement)>> _orderContainerFoodMap = new();
        private readonly Dictionary<IOrder, VisualElement> _orderContainerMap = new();

        protected override void OnInit()
        {
            _localizer = ServiceLocator.Current.Get<ILocalizer>();
            
            _timerLabel = Root.Q<Label>(name: _timerName);
            _scoreLabel = Root.Q<Label>(name: _scoreLabelName);
            _ordersContainer = Root.Q<VisualElement>(name: _ordersContainerName);

            _ordersTakenLabel = Root.Q<Label>(name: _ordersTakenValueName);
            _ordersCompletedLabel = Root.Q<Label>(name: _ordersCompletedValueName);
            _ordersFailedLabel = Root.Q<Label>(name: _ordersFailedValueName);

            _ordersContainer.Clear();
        }

        protected override void OnBind(HUDViewModel viewModel)
        {
            base.OnBind(viewModel);
            
            _disposables = new CompositeDisposable();
            
            viewModel.OnTimerTextChanged.Subscribe(x => _timerLabel.text = x.ToString()).AddTo(_disposables);
            viewModel.OnScoreChanged.Subscribe(x => _scoreLabel.text = x.ToString()).AddTo(_disposables);

            viewModel.ActiveOrders.ObserveAdd().Subscribe(x => OnOrderAdded(x.Value)).AddTo(_disposables);
            viewModel.ActiveOrders.ObserveRemove().Subscribe(x => OnOrderRemoved(x.Value)).AddTo(_disposables);

            viewModel.OrdersTaken.Subscribe(x => _ordersTakenLabel.text = x.ToString()).AddTo(_disposables);
            viewModel.OrdersCompleted.Subscribe(x => _ordersCompletedLabel.text = x.ToString()).AddTo(_disposables);
            viewModel.OrdersFailed.Subscribe(x => _ordersFailedLabel.text = x.ToString()).AddTo(_disposables);
        }

        private void OnOrderAdded(IOrder order)
        {
            var orderElement = _orderAsset.CloneTree();
            var foodContainer = orderElement.Q<VisualElement>(name: _foodContainerName);
            
            foodContainer.Clear();
            _orderContainerMap[order] = orderElement;
            _ordersContainer.Add(orderElement);
            
            var orderTimer = orderElement.Q<VisualElement>(name: "RemainingTimeNormalized");
            order.RemainingTimeNormalized.Subscribe(x => orderTimer.style.width = Length.Percent(x * 100f));
            
            order.PropagateHashes(x => AddOrderItem(foodContainer, ViewModel.MenuItemsHashMap[x], x));
            order.OnHashRemoved.Subscribe(x => RemoveOrderItem(foodContainer, x)); // dispose is redundant
        }

        private void RemoveOrderItem(VisualElement orderContainer, int hash)
        {
            if (!_orderContainerFoodMap.TryGetValue(orderContainer, out var hashedFoodList))
                return;

            var foodToRemove = hashedFoodList.FirstOrDefault(x => x.Item1 == hash);
            if (foodToRemove != default)
            {
                foodToRemove.Item2.RemoveFromHierarchy();
                hashedFoodList.Remove(foodToRemove);
            }
            else
            {
                FLogger.LogWarning<HUDView>($"Food {hash} not found");
            }
        }

        private void AddOrderItem(VisualElement orderContainer, ItemInfoSO itemData, int hash)
        {
            var foodItem = _foodItemAsset.CloneTree();
            orderContainer.Add(foodItem);
            
            var image = foodItem.Q<VisualElement>(name: _foodImageName);
            image.style.backgroundImage = new StyleBackground(itemData.Image);
            
            var nameLabel =  foodItem.Q<Label>(name: _foodLabelName);
            nameLabel.text = itemData.NameLid;
            _localizer.Localize(nameLabel, Constants.ITEMS_TABLE);

            if (_orderContainerFoodMap.TryGetValue(orderContainer, out var foodList))
            {
                foodList.Add((hash, foodItem));
            }
            else
            {
                foodList = new List<(int, VisualElement)> { (hash, foodItem) };
                _orderContainerFoodMap.Add(orderContainer, foodList);
            }
        }

        private void OnOrderRemoved(IOrder order)
        {
            if (_orderContainerMap.TryGetValue(order, out var element))
            {
                element.RemoveFromHierarchy();
                _orderContainerMap.Remove(order);
                _orderContainerFoodMap.Remove(element);
                element.RemoveFromHierarchy();
            }
        }

        public override void Dispose()
        {
            Disposes.ClearDispose(ref _disposables);
            base.Dispose();
        }
    }
}