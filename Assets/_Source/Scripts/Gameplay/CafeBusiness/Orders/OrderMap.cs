using System;
using System.Collections.Generic;

namespace ITCafe.CafeBusiness
{
    public class OrderMap : IOrder
    {
        public IReadOnlyDictionary<IOrderItem, int> OrderedItemsMap => _orderedItemsMap;
        public int OrderHash { get; }

        private readonly Dictionary<IOrderItem, int> _orderedItemsMap;

        public OrderMap(Dictionary<IOrderItem, int> orderedItemsMap)
        {
            _orderedItemsMap = orderedItemsMap;
            OrderHash = HashCode.Combine(orderedItemsMap);
        }
    }
}