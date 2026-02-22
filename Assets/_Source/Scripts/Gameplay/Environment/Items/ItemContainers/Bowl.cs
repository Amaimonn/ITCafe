using System;
using System.Collections.Generic;
using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Environment
{
    public class Bowl : ContainerItem, ISimpleContainer
    {
        public ItemTag ContainerTag => ItemTag.Bowl;
        public override IEnumerable<IItem> Items => _currentItems;
        
        [SerializeField, Min(0)] private float _itemsOffsetY = 0f;

        private readonly IItem[] _currentItems = new IItem[1];
        private readonly IMenuAspect[] _currentMenuAspects  = new IMenuAspect[1];
        
        private bool HasItem => CurrentItem != null;

        private IItem CurrentItem
        {
            get => _currentItems[0];
            set => _currentItems[0] = value;
        }

        private IMenuAspect CurrentMenuAspect
        {
            get => _currentMenuAspects[0];
            set => _currentMenuAspects[0] = value;
        }

        public override int GetItemHash()
        {
            if (HasItem)
                return CurrentMenuAspect.GetItemHash();

            var hash = new HashCode();
            
            return hash.ToHashCode();
        }

        public override bool CanTake(IItem item)
        {
            return !HasItem && 
                   item.TryGetCachedComponent<IMenuAspect>(out var menuItem) && 
                   menuItem.CanBeStored(this);
        }

        public override void Take(IItem item)
        {
            if (!item.TryGetCachedComponent<IMenuAspect>(out var menuItem))
                return;
            
            item.SetPhysicsEnabled(false);
            item.transform.SetParent(transform);
            item.transform.SetLocalPositionAndRotation(new Vector3(0, _itemsOffsetY, 0),
                Quaternion.identity);

            CurrentItem = item;
            CurrentMenuAspect = menuItem;
        }

        public override bool ContainsHash(int hash)
        {
            return HasItem && CurrentMenuAspect.GetItemHash() == hash;
        }

        public override IItem ExtractItem(int hash)
        {
            if (!HasItem ||  CurrentMenuAspect.GetItemHash() != hash)
                return null;
            
            var item = CurrentItem;
            CurrentItem = null;
            CurrentMenuAspect = null;

            return item;
        }
    }
}