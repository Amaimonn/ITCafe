using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace ITCafe.CafeBusiness
{
    public class WorkProgressService
    {
        public Observable<int> OnClientServed => _onClientServed;
        public Observable<int> OnOrderTaken => _onOrderTaken;
        public Observable<Unit> OnDayCompleted => _onDayCompleted;
        
        public float SuccessRate => _totalOrdersTaken > 0 ? (float)_successfulOrders / _totalOrdersTaken : 0f;
        public float AverageServiceTime => _totalClientsServed > 0 ? _totalServiceTime / _totalClientsServed : 0f;

        private readonly Dictionary<int, int> _itemsServedCountMap = new();
        private int _totalClientsServed = 0;
        private int _totalOrdersTaken = 0;
        private int _successfulOrders = 0;
        private int _failedOrders = 0;
        private float _totalServiceTime = 0f;
        private DateTime _dayStartTime = DateTime.Now;

        private readonly Subject<int> _onClientServed = new();
        private readonly Subject<int> _onOrderTaken = new();
        private readonly Subject<Unit> _onDayCompleted = new();

        public void RegisterClient(ClientCharacter client)
        {
            // TODO: watch out for subscriptions in object pool case
            client.OnOrdered.Subscribe(_ => OnOrderTakenHandler());
            client.OnCompleted.Subscribe(_ => OnOrderCompletedHandler(true)); // TODO: failure
            
            client.CurrentOrder.OnHashRemoved.Subscribe(RecordItemServed);
        }

        private void OnOrderTakenHandler()
        {
            _totalOrdersTaken++;
            _onOrderTaken.OnNext(_totalOrdersTaken);

            Debug.Log($"[Progress] Order taken. Total: {_totalOrdersTaken}");
        }

        private void OnOrderCompletedHandler(bool success)
        {
            if (success)
            {
                _successfulOrders++;
                _totalClientsServed++;
                _onClientServed.OnNext(_totalClientsServed);

                // _totalServiceTime += serviceTime;

                Debug.Log($"[{nameof(WorkProgressService)}]: Order completed successfully. Total served: {_totalClientsServed}");
            }
            else
            {
                _failedOrders++;
                Debug.Log($"[{nameof(WorkProgressService)}]: Order failed. Failed: {_failedOrders}");
            }
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
            _totalClientsServed = 0;
            _totalOrdersTaken = 0;
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
                ClientsServed = _totalClientsServed,
                OrdersTaken = _totalOrdersTaken,
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