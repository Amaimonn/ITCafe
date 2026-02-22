using System;
using System.Collections.Generic;
using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Environment
{
    public class Tray : ContainerItem, ISimpleContainer
    {
        public ItemTag ContainerTag => ItemTag.Tray;
        public override IEnumerable<IItem> Items => _currentItems;
        
        [SerializeField, Min(0)] private int _maxItemsCapacity = 4;
        [SerializeField, Min(0)] private float _itemsOffsetY = 0.15f;

        private readonly List<IItem> _currentItems = new();
        private readonly List<IMenuAspect> _currentMenuItems = new();
        private int _currentItemsAmount = 0;

        public override int GetItemHash()
        {
            var hash = new HashCode();

            if (_currentItemsAmount == 1)
                return _currentMenuItems[0].GetItemHash();

            foreach (var menuItem in _currentMenuItems)
                hash.Add(menuItem.GetItemHash());

            return hash.ToHashCode();
        }

        public override bool CanTake(IItem item)
        {
            return _currentItemsAmount < _maxItemsCapacity && 
                   item.TryGetCachedComponent<IMenuAspect>(out var menuItem) && 
                   menuItem.CanBeStored(this);
        }

        public override void Take(IItem item)
        {
            if (!item.TryGetCachedComponent<IMenuAspect>(out var menuItem))
                return;
            
            item.SetPhysicsEnabled(false);
            item.transform.SetParent(transform);
            item.transform.SetLocalPositionAndRotation(new Vector3(0, _currentItemsAmount * _itemsOffsetY, 0),
                Quaternion.identity);

            _currentItems.Add(item);
            _currentMenuItems.Add(menuItem);
            _currentItemsAmount++;
        }

        public override bool ContainsHash(int hash)
        {
            foreach (var menuItem in _currentMenuItems)
                if (menuItem.GetItemHash() == hash)
                    return true;

            return false;
        }

        public override IItem ExtractItem(int hash)
        {
            for (var i = 0; i < _currentItemsAmount; i++)
            {
                var item = _currentItems[i];
                var menuItem = _currentMenuItems[i];
                
                if (menuItem.GetItemHash() != hash)
                    continue;

                for (var j = i + 1; j < _currentItemsAmount; j++)
                    _currentItems[j].transform.localPosition -= new Vector3(0, _itemsOffsetY, 0);

                _currentItems.RemoveAt(i);
                _currentMenuItems.RemoveAt(i);
                _currentItemsAmount--;

                return item;
            }

            return null;
        }
    }
}