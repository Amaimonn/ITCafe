using System;
using System.Collections.Generic;
using R3;
using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using ObservableCollections;
using System.Threading;
using DevKit.UI.MVVM.Bases;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class HUDViewModel : ScreenViewModel
    {
        public Observable<string> TimerText => _timerText;
        public Observable<int> PointsAmount => _pointsAmount;

        public ReactiveProperty<int> OrdersTaken => _ordersTaken;
        public ReactiveProperty<int> OrdersCompleted => _ordersCompleted;
        public ReactiveProperty<int> OrdersFailed => _ordersFailed;


        public IObservableCollection<IOrder> ActiveOrders => _activeOrders;
        public IReadOnlyDictionary<int, ItemInfoSO> ItemInfoMap => _itemInfoMap;

        private readonly IReadOnlyDictionary<int, ItemInfoSO> _itemInfoMap;
        private readonly ObservableHashSet<IOrder> _activeOrders = new();
        private readonly ReactiveProperty<string> _timerText = new("00:00");
        private readonly ReactiveProperty<int> _pointsAmount = new(0);
        private readonly ReactiveProperty<int> _ordersTaken = new(0);
        private readonly ReactiveProperty<int> _ordersCompleted = new(0);
        private readonly ReactiveProperty<int> _ordersFailed = new(0);


        private CancellationTokenSource _timerCts;
        private DateTime _sessionStartTime;
        private IDisposable _timerSubscription;

        public HUDViewModel(IReadOnlyDictionary<int, ItemInfoSO> itemInfoMap)
        {
            _itemInfoMap = itemInfoMap;
        }

        public void SetRemainingSeconds(int remainingSeconds)
        {
            if (remainingSeconds <= 0)
                remainingSeconds = 0;
            
            _timerText.Value = FormatTime(TimeSpan.FromSeconds(remainingSeconds));
        }
     
        public void IncrementOrdersTaken()
        {
            _ordersTaken.Value++;
        }

        public void IncrementOrdersCompleted()
        {
            _ordersCompleted.Value++;
        }

        public void IncrementOrdersFailed()
        {
            _ordersFailed.Value++;
        }

        public void AddOrderInfo(IOrder order)
        {
            _activeOrders.Add(order);
        }

        public void RemoveOrderInfo(IOrder order)
        {
            _activeOrders.Remove(order);
        }

        public void SetPoints(int points)
        {
            _pointsAmount.Value = points;
        }

        public override void Dispose()
        {
            Disposes.ClearDispose(ref _timerSubscription);
            base.Dispose();
        }
        
        private string FormatTime(TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"mm\:ss");
        }
    }
}