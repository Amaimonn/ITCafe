using System.Collections.Generic;
using System.Linq;

namespace ITCafe.CafeBusiness
{
    public class OrderMap : IOrder
    {
        public IReadOnlyDictionary<int, int> OrderedItemsMap => _orderedItemsMap;
        public bool IsCompleted { get; private set; }
        public IEnumerable<int> OrderHashes => _orderedItemsMap.Keys;

        private readonly Dictionary<int, int> _orderedItemsMap; // key: hash, value: amount

        public OrderMap(Dictionary<IOrderItem, int> orderedItemsMap)
        {
            _orderedItemsMap = orderedItemsMap.ToDictionary(kvp => kvp.Key.OrderedItemHash, kvp => kvp.Value);
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

            return true;
        }
    }
}