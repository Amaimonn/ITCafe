using System;
using System.Collections.Generic;
using ITCafe.CafeBusiness;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public class Tray : PickUpItem, IItemsContainer
    {
        [SerializeField, Min(0)] private int _maxItemsCapacity = 4;
        [SerializeField, Min(0)] private float _itemsOffsetY = 0.15f;
        private List<IMenuItem> _currentItems = new();
        private int _currentItemsAmount = 0;

        public override bool CanInteract(PlayerContext context)
        {
            return context.ItemPicker.CanTake(this);
        }

        public override void Interact(PlayerContext context)
        {
            SetPhysicsEnabled(false);
            context.ItemPicker.Take(this);
        }

        public int GetItemHash()
        {
            var hash = new HashCode();

            if (_currentItemsAmount == 1)
                return _currentItems[0].GetItemHash();

            foreach (var item in _currentItems)
                hash.Add(item.GetItemHash());

            return hash.ToHashCode();
        }

        public bool CanTake(IItem item)
        {
            return _currentItemsAmount < _maxItemsCapacity;
        }

        public void Take(IMenuItem item)
        {
            item.SetPhysicsEnabled(false);
            item.transform.SetParent(transform);
            item.transform.SetLocalPositionAndRotation(new Vector3(0, _currentItemsAmount * _itemsOffsetY, 0),
                Quaternion.identity);
                
            _currentItems.Add(item);
            _currentItemsAmount++;
        }

        public bool ContainsHash(int hash)
        {
            foreach (var item in _currentItems)
                if (item.GetItemHash() == hash)
                    return true;

            return false;
        }

        public IItem ExtractItem(int hash)
        {
            for (var i = 0; i < _currentItemsAmount; i++)
            {
                var item = _currentItems[i];

                if (item.GetItemHash() == hash)
                {
                    for (var j = i + 1; j < _currentItemsAmount; j++)
                        _currentItems[j].transform.localPosition -= new Vector3(0, _itemsOffsetY, 0);

                    _currentItems.Remove(item);
                    _currentItemsAmount--;

                    return item;
                }
            }
            return null;
        }
    }
}