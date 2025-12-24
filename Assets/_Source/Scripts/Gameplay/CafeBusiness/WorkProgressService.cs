using System;
using System.Collections.Generic;
using DevKit.Utils;
using ITCafe.Data.Items;
using R3;

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


        private readonly IReadOnlyDictionary<int, ItemInfoSO> _itemConfigsMap;

        private readonly Dictionary<int, int> _itemsServedCountMap = new();
        private TimeSpan _totalTime;
        private int _totalClientsCount = 0;
        private int _successfulOrders = 0;
        private int _failedOrders = 0;
        private float _totalServiceTime = 0f;
        private DateTime _dayStartTime = DateTime.Now;
        private readonly int[] _fiveStarEvaluations = new int[5] { 200, 400, 600, 800, 1000 };
        private readonly Subject<int> _onOrderTaken = new();
        private readonly Subject<int> _onClientServed = new();
        private readonly Subject<int> _onClientFailed = new();
        private readonly Subject<Unit> _onDayCompleted = new();

        private const int SUCCESS_POINTS = 50;
        private const int FAILURE_POINTS = 50;

        public WorkProgressService(IReadOnlyDictionary<int, ItemInfoSO> itemConfigsMap)
        {
            _itemConfigsMap = itemConfigsMap;
        }

        public void RegisterClient(ClientCharacter client)
        {
            // TODO: watch out for subscriptions in object pool case
            client.OnOrdered.Subscribe(_ => OnOrderTakenHandler());
            client.OnCompleted.Subscribe(_ => OnOrderCompletedHandler());
            client.OnFailed.Subscribe(_ => OnOrderFailedHandler());
            client.CurrentOrder.OnHashRemoved.Subscribe(RecordItemServed);
        }

        public void SetTotalTime(TimeSpan totalTime)
        {
            _totalTime =  totalTime;
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
            _totalTime = TimeSpan.Zero;
            _totalClientsCount = 0;
            _successfulOrders = 0;
            _failedOrders = 0;
            _totalServiceTime = 0f;
            _dayStartTime = DateTime.Now;
            _itemsServedCountMap.Clear();
        }

        public ProgressReport GetDailyReport()
        {
            var points = CalcPoints();
            var stars = CalcStars(points);

            return new ProgressReport
            {
                WorkTime = _totalTime,
                DayStartTime = _dayStartTime,
                ClientsCount = _totalClientsCount,
                SuccessfulOrders = _successfulOrders,
                FailedOrders = _failedOrders,
                SuccessRate = SuccessRate,
                AverageServiceTime = AverageServiceTime,
                ItemsServed = _itemsServedCountMap,
                Points = points,
                EarnedStars = stars,
                StarEvaluations =  _fiveStarEvaluations,
            };
        }

        private int CalcPoints()
        {
            var points = _successfulOrders * SUCCESS_POINTS - _failedOrders * FAILURE_POINTS;
            foreach (var (hash, amount) in _itemsServedCountMap)
                points += _itemConfigsMap[hash].Points * amount;

            return points;
        }

        private int CalcStars(int points)
        {
            var starsAmount = 0;
            foreach (var starEvaluation in _fiveStarEvaluations)
                if (points >= starEvaluation)
                    starsAmount++;

            return starsAmount;
        }
    }
}