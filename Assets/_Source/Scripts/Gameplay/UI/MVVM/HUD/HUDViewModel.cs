using System;
using System.Collections.Generic;
using DevKit.UI.MVVM;
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
        private bool _isSessionRunning;
        private int _remainingSeconds;
        private IDisposable _timerSubscription;

        public HUDViewModel(IReadOnlyDictionary<int, ItemInfoSO> itemInfoMap)
        {
            _itemInfoMap = itemInfoMap;
        }

        public void StartSessionTimer(TimeSpan sessionDuration)
        {
            _remainingSeconds = (int)sessionDuration.TotalSeconds;
            _isSessionRunning = true;
            _timerSubscription = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1), UnityTimeProvider.Update)
                .TakeWhile(_ => _isSessionRunning)
                .Subscribe(_ => TimerDisplayTick());
        }

        public void StopSessionTimer()
        {
            _isSessionRunning = false;
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

        private void TimerDisplayTick()
        {
            _remainingSeconds--;

            if (_remainingSeconds <= 0)
            {
                _remainingSeconds = 0;
                _isSessionRunning = false;
            }

            _timerText.Value = FormatTime(TimeSpan.FromSeconds(_remainingSeconds));
        }

        private string FormatTime(TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"mm\:ss");
            // timeSpan.Hours > 0 
            // ? timeSpan.ToString(@"hh\:mm\:ss")
            // : timeSpan.ToString(@"mm\:ss");
        }
    }
}