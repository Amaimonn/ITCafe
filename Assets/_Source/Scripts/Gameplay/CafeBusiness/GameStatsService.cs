using System;
using System.Collections.Generic;
using DevKit.Utils;
using ITCafe.Data.Items;
using ITCafe.Data;
using R3;
using VContainer;

namespace ITCafe.CafeBusiness
{
    public class GameStatsService
    {
        public Observable<int> OnScoreChanged => _score;
        public Observable<int> OnOrderTaken => _onOrderTaken;
        public Observable<int> OnClientServed => _onClientServed;
        public Observable<int> OnClientFailed => _onClientFailed;
        public Observable<Unit> OnDayCompleted => _onDayCompleted;

        public float SuccessRate => _totalClientsCount > 0 ? (float)_successfulOrders / _totalClientsCount : 0f;
        public float AverageServiceTime => _totalClientsCount > 0 ? _totalServiceTime / _totalClientsCount : 0f;


        private readonly IReadOnlyDictionary<int, ItemInfoSO> _menuItemsHashMap;
        private readonly IReadOnlyList<int> _starEvaluations;

        private readonly Dictionary<int, int> _itemsServedCountMap = new();
        private TimeSpan _totalTime;
        private int _totalClientsCount = 0;
        private int _successfulOrders = 0;
        private int _failedOrders = 0;
        private float _totalServiceTime = 0f;
        private DateTime _dayStartTime = DateTime.Now;
        private readonly Subject<int> _onOrderTaken = new();
        private readonly Subject<int> _onClientServed = new();
        private readonly Subject<int> _onClientFailed = new();
        private readonly Subject<Unit> _onDayCompleted = new();
        private readonly ReactiveProperty<int> _score = new();

        private const int SUCCESS_POINTS = 50;
        private const int FAILURE_POINTS = 50;
        private ProgressReport? _cachedReport;

        public GameStatsService(IMissionEvaluation missionEvaluation,
            [Key(Constants.MENU_ITEMS_HASH_MAP)] IReadOnlyDictionary<int, ItemInfoSO> menuItemsHashMap)
        {
            _menuItemsHashMap = menuItemsHashMap;
            _starEvaluations = missionEvaluation.StarEvaluations;
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
            _totalTime = totalTime;
        }

        private void OnOrderTakenHandler()
        {
            _totalClientsCount++;
            _onOrderTaken.OnNext(_totalClientsCount);

            FLogger.Log<GameStatsService>($"Order has been taken. Total clients: {_totalClientsCount}");
        }

        private void OnOrderCompletedHandler()
        {
            _successfulOrders++;
            _onClientServed.OnNext(_successfulOrders);
            
            _score.Value += SUCCESS_POINTS;

            // _totalServiceTime += serviceTime;

            FLogger.Log<GameStatsService>($"Order completed successfully. Successful: {_successfulOrders}");
        }

        private void OnOrderFailedHandler()
        {
            _failedOrders++;
            _onClientFailed.OnNext(_failedOrders);
            
            _score.Value -= FAILURE_POINTS;
            
            FLogger.Log<GameStatsService>($"Order failed. Failed: {_failedOrders}");
        }

        public void RecordItemServed(int itemHash)
        {
            _itemsServedCountMap.TryAdd(itemHash, 0);
            _itemsServedCountMap[itemHash]++;
            
            _score.Value += _menuItemsHashMap[itemHash].MenuItemExtra.Points;
        }

        public void CompleteDay()
        {
            _onDayCompleted.OnNext(Unit.Default);
        }

        public void Reset()
        {
            _totalTime = TimeSpan.Zero;
            _totalClientsCount = 0;
            _successfulOrders = 0;
            _failedOrders = 0;
            _totalServiceTime = 0f;
            _dayStartTime = DateTime.Now;
            _itemsServedCountMap.Clear();
            _cachedReport = null;
        }

        public ProgressReport GetDailyReport()
        {
            if (_cachedReport != null)
                return _cachedReport.Value;

            var score = CalcScore();
            var stars = CalcStars(score);

            _cachedReport = new ProgressReport
            {
                WorkTime = _totalTime,
                DayStartTime = _dayStartTime,
                ClientsCount = _totalClientsCount,
                SuccessfulOrders = _successfulOrders,
                FailedOrders = _failedOrders,
                SuccessRate = SuccessRate,
                AverageServiceTime = AverageServiceTime,
                ItemsServed = _itemsServedCountMap,
                Score = _score.Value,
                EarnedStars = stars,
                StarEvaluations = _starEvaluations,
            };
            return _cachedReport.Value;
        }

        private int CalcScore()
        {
            return _score.Value;
        }

        private int CalcStars(int points)
        {
            var starsAmount = 0;
            
            foreach (var starEvaluation in _starEvaluations)
                if (points >= starEvaluation)
                    starsAmount++;

            return starsAmount;
        }
    }
}