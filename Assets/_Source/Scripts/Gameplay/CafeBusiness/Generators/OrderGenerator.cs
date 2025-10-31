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
        private int _maxOrderSizeExclusive = 5;

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
                int orderSize = Random.Range(1, _maxOrderSizeExclusive);
                
                if (orderSize == 1)
                {
                    var randomItem = _itemInfos[Random.Range(0, itemCount)];
                    var itemHash = randomItem.ItemInfo.GetItemHash();

                    order = new OrderItem(itemHash);
                }
                else
                {
                    Dictionary<int, int> orderedItemsMap = new();

                    for (int i = 0; i < orderSize; i++)
                    {
                        var randomItem = _itemInfos[Random.Range(0, itemCount)];
                        var hash = randomItem.ItemInfo.GetItemHash();
                        
                        if (!orderedItemsMap.TryAdd(hash, 1))
                            orderedItemsMap[hash] += 1;
                    }
                    
                    order = new OrderMap(orderedItemsMap);
                }
            }
            else
            {
                Debug.LogError($"No items available for order generation");
                order = new OrderItem(-1);
            }

            return order;
        }
    }
}