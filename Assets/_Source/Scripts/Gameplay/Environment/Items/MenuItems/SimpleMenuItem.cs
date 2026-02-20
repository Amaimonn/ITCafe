using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Environment
{
    /// <summary>
    /// Final food item that can be used in orders.
    /// Caution: it can`t be used in crafting now (it doesn`t implement ICraftItem)
    /// </summary>
    public class SimpleMenuItem : PickUpItem, IMenuItem
    {
        public string Id { get; set; }

        [SerializeField] private SimpleItemInfo _info = new();

        public int GetItemHash()
        {
            return _info.GetItemHash();
        }
    }
}