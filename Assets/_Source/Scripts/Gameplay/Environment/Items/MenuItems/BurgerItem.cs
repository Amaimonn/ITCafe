using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Environment
{
    public class BurgerItem : PickUpItem, IMenuItem
    {
        public string Id { get; set; }

        [SerializeField] private BurgerItemInfo _info = new();

        public int GetItemHash()
        {
            return _info.GetItemHash();
        }

        public bool CanBeStored(IItemsContainer container)
        {
            throw new System.NotImplementedException();
        }
    }
}