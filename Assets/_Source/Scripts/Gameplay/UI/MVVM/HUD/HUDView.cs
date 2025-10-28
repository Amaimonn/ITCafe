using MiUI.MVVM;
using UnityEngine;
using UnityEngine.UIElements;
using R3;
using System.Collections.Generic;
using System.Linq;
using ITCafe.CafeBusiness;
using ObservableCollections;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class HUDView : ToolkitView<HUDViewModel>
    {
        [SerializeField] private string _timerName = "Timer";
        [SerializeField] private string _pointsLabelName = "PointsLabel";
        [SerializeField] private string _ordersContainerName = "OrdersContainer";
        [SerializeField] private string _ordersTakenValueName = "OrderValue";
        [SerializeField] private string _ordersCompletedValueName = "SatisfactionValue";
        [SerializeField] private string _ordersFailedValueName = "FailedValue";

        private Label _timerLabel;
        private Label _pointsLabel;
        private Label _ordersTakenLabel;
        private Label _ordersCompletedLabel;
        private Label _ordersFailedLabel;
        private VisualElement _ordersContainer;

        private CompositeDisposable _disposables;

        private readonly Dictionary<VisualElement, List<(int, VisualElement)>> _orderContainerImagesMap = new();
        private readonly Dictionary<IOrder, VisualElement> _orderContainerMap = new();

        protected override void OnInit()
        {
            _timerLabel = Root.Q<Label>(name: _timerName);
            _pointsLabel = Root.Q<Label>(name: _pointsLabelName);
            _ordersContainer = Root.Q<VisualElement>(name: _ordersContainerName);
            
            _ordersTakenLabel =  Root.Q<Label>(name: _ordersTakenValueName);
            _ordersCompletedLabel = Root.Q<Label>(name: _ordersCompletedValueName);
            _ordersFailedLabel = Root.Q<Label>(name: _ordersFailedValueName);
            
            _ordersContainer.Clear();
        }

        protected override void OnBind(HUDViewModel viewModel)
        {
            _disposables = new CompositeDisposable
            {
                viewModel.TimerText.Subscribe(x => _timerLabel.text = x.ToString()),
                viewModel.PointsAmount.Subscribe(x => _pointsLabel.text = x.ToString()),

                viewModel.ActiveOrders.ObserveAdd().Subscribe(x => OnOrderAdded(x.Value)),
                viewModel.ActiveOrders.ObserveRemove().Subscribe(x => OnOrderRemoved(x.Value)),
                
                viewModel.OrdersTaken.Subscribe(x => _ordersTakenLabel.text = x.ToString()),
                viewModel.OrdersCompleted.Subscribe(x => _ordersCompletedLabel.text = x.ToString()),
                viewModel.OrdersFailed.Subscribe(x => _ordersFailedLabel.text = x.ToString()),
            };
        }
        
        private void OnOrderAdded(IOrder order)
        {
            var orderContainer = new VisualElement();
            orderContainer.AddToClassList("hud__order");
            _orderContainerMap[order] = orderContainer;
            _ordersContainer.Add(orderContainer);

            order.PropagateHashes(x => AddOrderImage(orderContainer, ViewModel.ItemInfoMap[x].Image, x));
            order.OnHashRemoved.Subscribe(x => RemoveOrderImage(orderContainer, x)); // dispose is redundant
        }

        private void RemoveOrderImage(VisualElement orderContainer, int hash)
        {
            if (!_orderContainerImagesMap.TryGetValue(orderContainer, out var hashedImagesList))
                return;

            var imageToRemove = hashedImagesList.FirstOrDefault(x => x.Item1 == hash);
            if (imageToRemove != default)
            {
                imageToRemove.Item2.RemoveFromHierarchy();
                hashedImagesList.Remove(imageToRemove);
            }
            else
                Debug.LogWarning($"Image {hash} not found");
        }

        private void AddOrderImage(VisualElement orderContainer, Sprite sprite, int hash)
        {
            var image = new VisualElement
            {
                style = { backgroundImage = new StyleBackground(sprite) },
                name = hash.ToString()
            };
            image.AddToClassList("order-cloud__item-image");
            orderContainer.Add(image);

            if (_orderContainerImagesMap.TryGetValue(orderContainer, out var imageList))
            {
                imageList.Add((hash, image));
            }
            else
            {
                imageList = new List<(int, VisualElement)> { (hash, image) };
                _orderContainerImagesMap.Add(orderContainer, imageList);
            }
        }

        private void OnOrderRemoved(IOrder order)
        {
            if (_orderContainerMap.TryGetValue(order, out var element))
            {
                element.RemoveFromHierarchy();
                _orderContainerMap.Remove(order);
                _orderContainerImagesMap.Remove(element);
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