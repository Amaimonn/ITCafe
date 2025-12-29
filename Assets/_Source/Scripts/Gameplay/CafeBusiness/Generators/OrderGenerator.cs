using System.Collections.Generic;
using System.Linq;
using DevKit.Solutions;
using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using UnityEngine;
using VContainer;

namespace ITCafe.Gameplay.CafeBusiness
{
    public class OrderGenerator : IFactory<IOrder>
    {
        private readonly ItemInfoSO[] _menuItemsInfo;
        private readonly int _itemAmount;
        private const int MAX_ORDER_SIZE_EXCLUSIVE = 5;
        private const float BASE_ORDER_TIME = 60f;
        private const float COMPLEXITY_TIME = 10f;

        public OrderGenerator([Key(Constants.MENU_ITEMS_MAP)] IReadOnlyDictionary<ItemTag, ItemInfoSO> menuItemMap)
        {
            _menuItemsInfo = menuItemMap.Values.ToArray();
            _itemAmount = _menuItemsInfo.Length;
        }

        public IOrder Create()
        {
            IOrder order;

            if (_itemAmount > 0)
            {
                var orderSize = Random.Range(1, MAX_ORDER_SIZE_EXCLUSIVE);
                var totalTime = BASE_ORDER_TIME;

                if (orderSize == 1)
                {
                    var itemHash = GetRandomHashAndCalcTime(ref totalTime);

                    order = new OrderItem(itemHash, totalTime);
                }
                else
                {
                    Dictionary<int, int> orderedItemsMap = new();
                    totalTime *= 1f + 0.1f * orderSize;

                    for (var i = 0; i < orderSize; i++)
                    {
                        var itemHash = GetRandomHashAndCalcTime(ref totalTime);
                        
                        if (!orderedItemsMap.TryAdd(itemHash, 1))
                            orderedItemsMap[itemHash] += 1;
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

        private int GetRandomHashAndCalcTime(ref float totalTime)
        {
            var randomItem = _menuItemsInfo[Random.Range(0, _itemAmount)];
            var hash = randomItem.MenuItemExtra.ItemInfo.GetItemHash();
            
            totalTime += randomItem.MenuItemExtra.ComplexityTimeModifier * COMPLEXITY_TIME;
            
            return hash;
        }
    }
}