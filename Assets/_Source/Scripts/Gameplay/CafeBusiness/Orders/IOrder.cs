using System;
using R3;

namespace ITCafe.CafeBusiness
{
    public interface IOrder
    {
        public Observable<int> OnHashRemoved { get; }
        public ReadOnlyReactiveProperty<float> RemainingTime { get; }
        public Observable<float> RemainingTimeNormalized { get; }
        public bool IsCompleted { get; }
        public float TotalTime { get; }

        public bool IsCorresponds(int hash);
        public void PropagateHashes(Action<int> onPropagate);
        public bool TryHandOver(int hash);
        public void TickTime(float deltaTime);
    }
}