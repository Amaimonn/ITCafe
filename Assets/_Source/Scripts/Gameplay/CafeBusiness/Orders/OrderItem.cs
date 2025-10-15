using System;

namespace ITCafe.CafeBusiness
{
    public class OrderItem : BaseOrder, IOrderItem
    {
        public int OrderedItemHash { get; }

        public OrderItem(int orderedItemHash)
        {
            OrderedItemHash = orderedItemHash;
        }

        public override bool IsCorresponds(int hash)
        {
            return OrderedItemHash == hash;
        }

        public override void PropagateHashes(Action<int> onPropagate)
        {
            onPropagate(OrderedItemHash);
        }

        public override bool TryHandOver(int hash)
        {
            if (hash != OrderedItemHash)
                return false;

            IsCompleted = true;
            _onHashRemoved.OnNext(OrderedItemHash);

            return true;
        }
    }
}