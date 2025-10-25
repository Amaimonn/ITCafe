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

        private Label _timerLabel;
        private Label _pointsLabel;
        private VisualElement _ordersContainer;

        private CompositeDisposable _disposables;

        private readonly Dictionary<VisualElement, List<(int, VisualElement)>> _orderContainerImagesMap = new();
        private readonly Dictionary<IOrder, VisualElement> _orderContainerMap = new();

        protected override void OnInit()
        {
            _timerLabel = Root.Q<Label>(_timerName);
            _pointsLabel = Root.Q<Label>(_pointsLabelName);
            _ordersContainer = Root.Q<VisualElement>(_ordersContainerName);
            _ordersContainer.Clear();
        }

        protected override void OnBind(HUDViewModel viewModel)
        {
            _disposables = new CompositeDisposable
            {
                viewModel.TimerText.Subscribe(x => _timerLabel.text = x),
                viewModel.PointsText.Subscribe(x => _pointsLabel.text = x),

                viewModel.ActiveOrders.ObserveAdd().Subscribe(x => OnOrderAdded(x.Value)),
                viewModel.ActiveOrders.ObserveRemove().Subscribe(x => OnOrderRemoved(x.Value)),
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
            _disposables?.Dispose();
            _disposables = null;
            base.Dispose();
        }
    }
}