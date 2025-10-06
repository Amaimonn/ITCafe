using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Environment
{
    public class SimpleItem : PickUpItem, IMenuItem
    {
        public string Id { get; set; }

        [SerializeField] private SimpleItemInfo _info = new();

        public int GetItemHash()
        {
            return _info.GetItemHash();
        }
    }
}