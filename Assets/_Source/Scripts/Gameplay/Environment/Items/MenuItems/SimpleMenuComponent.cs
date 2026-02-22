using ITCafe.Data.Items;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public class SimpleMenuComponent: PickUpItem, IMenuItem
    {
        [SerializeField] private SimpleItemInfo _info = new();
        [SerializeField] private ItemTag[] _properContainerTags;
        [SerializeField] private bool _emptyHandsStorable;
        
        public int GetItemHash()
        {
            return _info.GetItemHash();
        }

        public bool CanBeStored(IItemsContainer container)
        {
            return true; // TODO: check container is ISimpleContainer simple && simple.ContainerTag == _properTag
        }
    }
}