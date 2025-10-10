using System.Collections.Generic;
using System.Linq;

namespace ITCafe.CafeBusiness
{
    public class OrderMap : IOrder
    {
        public IReadOnlyDictionary<int, int> OrderedItemsMap => _orderedItemsMap;
        public bool IsCompleted { get; private set; }
        public IEnumerable<int> OrderHashes => _orderHashes;

        private Dictionary<int, int> _orderedItemsMap; // key: hash, value: amount
        private List<int> _orderHashes;

        public OrderMap(Dictionary<IOrderItem, int> orderedItemsMap)
        {
            _orderedItemsMap = orderedItemsMap.ToDictionary(kvp => kvp.Key.OrderedItemHash, kvp => kvp.Value);
            InitHashesCollection();
        }

        public OrderMap(Dictionary<int, int> orderedItemsMap)
        {
            _orderedItemsMap = orderedItemsMap;
            InitHashesCollection();
        }

        public static OrderMap FromEnumerable<T>(IEnumerable<T> orderedItems) where T : IOrderItem
        {
            Dictionary<int, int> orderedItemsMap = new();

            foreach (var orderedItem in orderedItems)
            {
                var hash = orderedItem.OrderedItemHash;
                if (orderedItemsMap.ContainsKey(hash))
                    orderedItemsMap[hash] += 1;
                else
                    orderedItemsMap[hash] = 1;
            }

            return new OrderMap(orderedItemsMap);
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

            _orderHashes.Remove(hash);
            return true;
        }

        private void InitHashesCollection()
        {
            _orderHashes = new List<int>();

            foreach (var orderedItem in _orderedItemsMap)
                for (var i = 0; i < orderedItem.Value; i++)
                    _orderHashes.Add(orderedItem.Key);
        }
    }
}