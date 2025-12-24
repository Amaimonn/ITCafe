using System;
using System.Collections.Generic;

namespace ITCafe.CafeBusiness
{
    public struct ProgressReport
    {
        public TimeSpan WorkTime { get; set; }
        public DateTime DayStartTime { get; set; }
        public int ClientsCount { get; set; }
        public int SuccessfulOrders { get; set; }
        public int FailedOrders { get; set; }
        public float SuccessRate { get; set; }
        public float AverageServiceTime { get; set; }
        public IReadOnlyDictionary<int, int> ItemsServed { get; set; }
        public int Points { get; set; }
        public int EarnedStars { get; set; }
        public IReadOnlyList<int> StarEvaluations { get; set; } 

        public override string ToString()
        {
            return $"Clients: {ClientsCount}, Success: {SuccessRate:P2}, Avg Time: {AverageServiceTime:F2}s";
        }
    }
}