using System;
using System.Collections.Generic;

namespace ITCafe.CafeBusiness
{
    public struct ProgressReport
    {
        public DateTime DayStartTime;
        public int ClientsServed;
        public int OrdersTaken;
        public int SuccessfulOrders;
        public int FailedOrders;
        public float SuccessRate;
        public float AverageServiceTime;
        public IReadOnlyDictionary<int, int> ItemsServed;

        public override string ToString()
        {
            return $"Clients: {ClientsServed}, Success: {SuccessRate:P2}, Avg Time: {AverageServiceTime:F2}s";
        }
    }
}