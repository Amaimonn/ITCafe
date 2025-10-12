using System.Collections.Generic;
using System.Linq;

namespace ITCafe.CafeBusiness
{
    public class OrderMap : IOrder
    {
        public IReadOnlyDictionary<int, int> OrderedItemsMap => _orderedItemsMap;
        public bool IsCompleted { get; private set; }

        private Dictionary<int, int> _orderedItemsMap; // key: hash, value: amount

        public OrderMap(Dictionary<IOrderItem, int> orderedItemsMap)
        {
            _orderedItemsMap = orderedItemsMap.ToDictionary(kvp => kvp.Key.OrderedItemHash, kvp => kvp.Value);
        }

        public OrderMap(Dictionary<int, int> orderedItemsMap)
        {
            _orderedItemsMap = orderedItemsMap;
        }

        public static OrderMap FromEnumerable<T>(IEnumerable<T> orderedItems) where T : IOrderItem
        {
            Dictionary<int, int> orderedItemsMap = new();

            foreach (var orderedItem in orderedItems)
            {
                var hash = orderedItem.OrderedItemHash;
                if (!orderedItemsMap.TryAdd(hash, 1))
                    orderedItemsMap[hash] += 1;
            }

            return new OrderMap(orderedItemsMap);
        }

        public bool IsCorresponds(int hash)
        {
            return _orderedItemsMap.ContainsKey(hash);
        }

        public bool TryHandOver(int hash)
        {
            if (!_orderedItemsMap.TryGetValue(hash, out var amount))
                return false;

            amount -= 1;

            if (amount == 0)
            {
                _orderedItemsMap.Remove(hash);
                if (_orderedItemsMap.Count == 0)
                    IsCompleted = true;
            }
            else
                _orderedItemsMap[hash] -= 1;

            return true;
        }
    }
}