using System;
using R3;
using UnityEngine;

namespace ITCafe.CafeBusiness
{
    public abstract class BaseOrder : IOrder
    {
        public Observable<int> OnHashRemoved => _onHashRemoved;
        public ReadOnlyReactiveProperty<float> RemainingTime => _totalTime;
        public Observable<float> RemainingTimeNormalized => _remainingTimeNormalized;
        public bool IsCompleted { get; protected set; }
        public float TotalTime { get; protected set; }
        
        protected readonly Subject<int> _onHashRemoved = new();
        protected readonly ReactiveProperty<float> _totalTime = new();
        protected readonly ReactiveProperty<float> _remainingTimeNormalized = new();
        
        public abstract bool IsCorresponds(int hash);
        public abstract void PropagateHashes(Action<int> onPropagate);
        public abstract bool TryHandOver(int hash);

        public BaseOrder(float totalTime)
        {
            TotalTime = totalTime;
            _totalTime.Value = totalTime;
            _remainingTimeNormalized.Value = totalTime;
        }
        
        public virtual void TickTime(float deltaTime)
        {
            _totalTime.Value = Mathf.Max(0f, _totalTime.Value - deltaTime);
            _remainingTimeNormalized.Value = _totalTime.Value / TotalTime;
        }
    }
}