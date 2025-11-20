using System;
using System.Collections.Generic;
using System.Linq;

namespace ITCafe.CafeBusiness
{
    public class OrderMap : BaseOrder
    {
        public IReadOnlyDictionary<int, int> OrderedItemsMap => _orderedItemsMap;

        private readonly Dictionary<int, int> _orderedItemsMap; // key: hash, value: amount

        public OrderMap(Dictionary<IOrderItem, int> orderedItemsMap, float totalTime) :  base(totalTime)
        {
            _orderedItemsMap = orderedItemsMap.ToDictionary(kvp => kvp.Key.OrderedItemHash, kvp => kvp.Value);
        }

        public OrderMap(Dictionary<int, int> orderedItemsMap, float totalTime) :  base(totalTime)
        {
            _orderedItemsMap = orderedItemsMap;
        }

        public static OrderMap FromEnumerable<T>(IEnumerable<T> orderedItems, float totalTime) where T : IOrderItem
        {
            Dictionary<int, int> orderedItemsMap = new();

            foreach (var orderedItem in orderedItems)
            {
                var hash = orderedItem.OrderedItemHash;
                if (!orderedItemsMap.TryAdd(hash, 1))
                    orderedItemsMap[hash] += 1;
            }

            return new OrderMap(orderedItemsMap, totalTime);
        }

        public override bool IsCorresponds(int hash)
        {
            return _orderedItemsMap.ContainsKey(hash);
        }

        public override void PropagateHashes(Action<int> onPropagate)
        {
            foreach (var orderPair in _orderedItemsMap)
                for (var i = 0; i < orderPair.Value; i++)
                    onPropagate(orderPair.Key);
        }

        public override bool TryHandOver(int hash)
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

            _onHashRemoved.OnNext(hash);

            return true;
        }
    }
}