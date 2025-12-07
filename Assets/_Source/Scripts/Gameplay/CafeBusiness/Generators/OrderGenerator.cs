using System.Collections.Generic;
using System.Linq;
using DevKit.Solutions;
using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Gameplay.CafeBusiness
{
    public class OrderGenerator : IFactory<IOrder>
    {
        private readonly List<ItemInfoSO> _itemInfos;
        private const int MAX_ORDER_SIZE_EXCLUSIVE = 5;
        private const float BASE_ORDER_TIME = 60f;
        private const float COMPLEXITY_TIME = 10f;

        public OrderGenerator(IEnumerable<ItemInfoSO> itemInfos)
        {
            _itemInfos = itemInfos.ToList();
        }

        public IOrder Create()
        {
            IOrder order;
            var itemCount = _itemInfos.Count;

            if (itemCount > 0)
            {
                var orderSize = Random.Range(1, MAX_ORDER_SIZE_EXCLUSIVE);
                var totalTime = BASE_ORDER_TIME;

                if (orderSize == 1)
                {
                    var randomItem = _itemInfos[Random.Range(0, itemCount)];
                    var itemHash = randomItem.ItemInfo.GetItemHash();
                    totalTime += randomItem.ComplexityTimeModifier * COMPLEXITY_TIME;

                    order = new OrderItem(itemHash, totalTime);
                }
                else
                {
                    Dictionary<int, int> orderedItemsMap = new();
                    totalTime *= 1f + 0.1f * orderSize;

                    for (var i = 0; i < orderSize; i++)
                    {
                        var randomItem = _itemInfos[Random.Range(0, itemCount)];
                        var hash = randomItem.ItemInfo.GetItemHash();
                        totalTime += randomItem.ComplexityTimeModifier * COMPLEXITY_TIME;
                        
                        if (!orderedItemsMap.TryAdd(hash, 1))
                            orderedItemsMap[hash] += 1;
                    }

                    order = new OrderMap(orderedItemsMap, totalTime);
                }
            }
            else
            {
                Debug.LogError($"No items available for order generation");
                order = new OrderItem(-1, 1f);
            }

            return order;
        }
    }
}