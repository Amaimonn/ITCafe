using ITCafe.Gameplay.Orders;
using UnityEngine;

namespace ITCafe
{
    public class BurgerItem : ProductItem, IMenuItem
    {
        public string Id { get; set; }

        private BurgerItemInfo _info;

        public int GetItemHash()
        {
            return _info.GetItemHash();
        }
    }
}