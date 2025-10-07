using System.Collections.Generic;

namespace ITCafe.CafeBusiness
{
    public class OrderItem : IOrderItem, IOrder
    {
        public bool IsCompleted { get; private set; }
        public IEnumerable<int> OrderHashes { get; }
        public int OrderedItemHash { get; set; }

        public OrderItem(int orderedItemHash)
        {
            OrderedItemHash = orderedItemHash;
            OrderHashes = new[] { orderedItemHash };
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