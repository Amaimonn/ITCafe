using System;
using System.Collections.Generic;
using ITCafe.CafeBusiness;
using UnityEngine;

namespace ITCafe.Environment
{
    public class Tray : ContainerItem
    {
        public override IEnumerable<IMenuItem> Items => _currentItems;
        
        [SerializeField, Min(0)] private int _maxItemsCapacity = 4;
        [SerializeField, Min(0)] private float _itemsOffsetY = 0.15f;

        private readonly List<IMenuItem> _currentItems = new();
        private int _currentItemsAmount = 0;

        public override int GetItemHash()
        {
            var hash = new HashCode();

            if (_currentItemsAmount == 1)
                return _currentItems[0].GetItemHash();

            foreach (var item in _currentItems)
                hash.Add(item.GetItemHash());

            return hash.ToHashCode();
        }

        public override bool CanTake(IMenuItem item)
        {
            return _currentItemsAmount < _maxItemsCapacity;
        }

        public override void Take(IMenuItem item)
        {
            item.SetPhysicsEnabled(false);
            item.transform.SetParent(transform);
            item.transform.SetLocalPositionAndRotation(new Vector3(0, _currentItemsAmount * _itemsOffsetY, 0),
                Quaternion.identity);

            _currentItems.Add(item);
            _currentItemsAmount++;
        }

        public override bool ContainsHash(int hash)
        {
            foreach (var item in _currentItems)
                if (item.GetItemHash() == hash)
                    return true;

            return false;
        }

        public override IMenuItem ExtractItem(int hash)
        {
            for (var i = 0; i < _currentItemsAmount; i++)
            {
                var item = _currentItems[i];

                if (item.GetItemHash() != hash)
                    continue;

                for (var j = i + 1; j < _currentItemsAmount; j++)
                    _currentItems[j].transform.localPosition -= new Vector3(0, _itemsOffsetY, 0);

                _currentItems.Remove(item);
                _currentItemsAmount--;

                return item;
            }

            return null;
        }
    }
}