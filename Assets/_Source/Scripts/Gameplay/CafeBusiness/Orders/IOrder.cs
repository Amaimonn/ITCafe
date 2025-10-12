using System;
using R3;

namespace ITCafe.CafeBusiness
{
    public interface IOrder
    {
        public Observable<int> OnHashRemoved { get; }
        public bool IsCompleted { get; }

        public bool IsCorresponds(int hash);
        public void PropagateHashes(Action<int> onPropagate);
        public bool TryHandOver(int hash);
    }
}