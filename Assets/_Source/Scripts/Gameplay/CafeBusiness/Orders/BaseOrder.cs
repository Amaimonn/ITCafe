using System;
using R3;

namespace ITCafe.CafeBusiness
{
    public abstract class BaseOrder : IOrder
    {
        public Observable<int> OnHashRemoved => _onHashRemoved;
        public bool IsCompleted { get; protected set; }
        
        protected readonly Subject<int> _onHashRemoved = new();

        public abstract bool IsCorresponds(int hash);

        public abstract void PropagateHashes(Action<int> onPropagate);
        public abstract bool TryHandOver(int hash);
    }
}