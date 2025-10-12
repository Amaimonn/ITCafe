using System.Collections.Generic;

namespace ITCafe.CafeBusiness
{
    public class OrderItem : IOrderItem, IOrder
    {
        public bool IsCompleted { get; private set; }
        public int OrderedItemHash { get; }

        public OrderItem(int orderedItemHash)
        {
            OrderedItemHash = orderedItemHash;
        }

        public bool IsCorresponds(int hash)
        {
            return OrderedItemHash == hash;
        }

        public bool TryHandOver(int hash)
        {
            if (hash != OrderedItemHash)
                return false;

            IsCompleted = true;

            return true;
        }
    }
}