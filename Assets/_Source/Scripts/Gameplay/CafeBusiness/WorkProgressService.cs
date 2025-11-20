using System;
using System.Collections.Generic;
using DevKit.Utils;
using R3;
using UnityEngine;

namespace ITCafe.CafeBusiness
{
    public class WorkProgressService
    {
        public Observable<int> OnOrderTaken => _onOrderTaken;
        public Observable<int> OnClientServed => _onClientServed;
        public Observable<int> OnClientFailed => _onClientFailed;
        public Observable<Unit> OnDayCompleted => _onDayCompleted;
        
        public float SuccessRate => _totalClientsCount > 0 ? (float)_successfulOrders / _totalClientsCount : 0f;
        public float AverageServiceTime => _totalClientsCount > 0 ? _totalServiceTime / _totalClientsCount : 0f;

        private readonly Dictionary<int, int> _itemsServedCountMap = new();
        private int _totalClientsCount = 0;
        private int _successfulOrders = 0;
        private int _failedOrders = 0;
        private float _totalServiceTime = 0f;
        private DateTime _dayStartTime = DateTime.Now;

        private readonly Subject<int> _onOrderTaken = new();
        private readonly Subject<int> _onClientServed = new();
        private readonly Subject<int> _onClientFailed = new();
        private readonly Subject<Unit> _onDayCompleted = new();

        public void RegisterClient(ClientCharacter client)
        {
            // TODO: watch out for subscriptions in object pool case
            client.OnOrdered.Subscribe(_ => OnOrderTakenHandler());
            client.OnCompleted.Subscribe(_ => OnOrderCompletedHandler());
            client.OnFailed.Subscribe(_ => OnOrderFailedHandler());
            client.CurrentOrder.OnHashRemoved.Subscribe(RecordItemServed);
        }

        private void OnOrderTakenHandler()
        {
            _totalClientsCount++;
            _onOrderTaken.OnNext(_totalClientsCount);

            FLogger.Log<WorkProgressService>($"Order has been taken. Total clients: {_totalClientsCount}");
        }

        private void OnOrderCompletedHandler()
        {
            _successfulOrders++;
            _onClientServed.OnNext(_successfulOrders);

            // _totalServiceTime += serviceTime;

            FLogger.Log<WorkProgressService>($"Order completed successfully. Successful: {_successfulOrders}");
        }

        private void OnOrderFailedHandler()
        {
            _failedOrders++;
            _onClientFailed.OnNext(_failedOrders);
            FLogger.Log<WorkProgressService>($"Order failed. Failed: {_failedOrders}");
        }

        public void RecordItemServed(int itemHash)
        {
            _itemsServedCountMap.TryAdd(itemHash, 0);
            _itemsServedCountMap[itemHash]++;
        }
        
        public void CompleteDay()
        {
            _onDayCompleted.OnNext(Unit.Default);
        }

        public void ResetDailyStats()
        {
            _totalClientsCount = 0;
            _successfulOrders = 0;
            _failedOrders = 0;
            _totalServiceTime = 0f;
            _dayStartTime = DateTime.Now;
            _itemsServedCountMap.Clear();
        }

        public ProgressReport GetDailyReport()
        {
            return new ProgressReport
            {
                DayStartTime = _dayStartTime,
                ClientsCount = _totalClientsCount,
                SuccessfulOrders = _successfulOrders,
                FailedOrders = _failedOrders,
                SuccessRate = SuccessRate,
                AverageServiceTime = AverageServiceTime,
                ItemsServed = _itemsServedCountMap,
                EarnedStars = 3 // TODO: Calc
            };
        }
    }
}