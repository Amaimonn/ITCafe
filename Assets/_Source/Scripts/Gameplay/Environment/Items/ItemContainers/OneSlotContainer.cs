using System;
using System.Collections.Generic;
using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Environment
{
    public abstract class OneSlotContainer : ContainerItem, ISimpleContainer, IMenuAspect
    {
        public abstract ItemTag ContainerTag { get; }
        public override IEnumerable<IItem> Items => _currentItems;

        [SerializeField, Min(0)] protected float _itemsOffsetY = 0f;

        private readonly IItem[] _currentItems = new IItem[1];
        private readonly IMenuAspect[] _currentMenuAspects = new IMenuAspect[1];

        protected bool HasItem => CurrentItem != null;

        protected IItem CurrentItem
        {
            get => _currentItems[0];
            set => _currentItems[0] = value;
        }

        protected IMenuAspect CurrentMenuAspect
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

#region IMenuAspect
        public bool CanBeStored(IItemsContainer container)
        {
            // empty container can`t be stored
            return HasItem && container is ISimpleContainer { ContainerTag: ItemTag.Tray }; // hardcoded
        }
#endregion

        /// <summary>
        /// Can only take menu items by default and if container has no items taken yet. 
        /// </summary>
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
            if (!HasItem || CurrentMenuAspect.GetItemHash() != hash)
                return null;

            var item = CurrentItem;
            CurrentItem = null;
            CurrentMenuAspect = null;

            return item;
        }
    }
}