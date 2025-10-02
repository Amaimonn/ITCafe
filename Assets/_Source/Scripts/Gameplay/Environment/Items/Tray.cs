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
        private List<IMenuItem> _currentItems = new();
        private int _currentItemsAmount = 0;

        public override bool CanInteract(PlayerContext context)
        {
            return context.ItemPicker.CanTake();
        }

        public override void Interact(PlayerContext context)
        {
            base.Interact(context);
            // TODO: change ItemPicker behaviour mb?
        }

        public int GetItemHash()
        {
            var hash = new HashCode();
            foreach (var item in _currentItems)
            {
                hash.Add(item.GetItemHash());
            }

            return hash.ToHashCode();
        }

        public bool CanTake()
        {
            return _currentItemsAmount < _maxItemsCapacity;
        }

        public void Take(IMenuItem item)
        {
            item.SetPhysicsEnabled(false);
            item.transform.SetParent(transform);
            item.transform.SetPositionAndRotation(
                -item.CenterOffset + new Vector3(0.15f - 0.1f * _currentItemsAmount, 0, 0),
                Quaternion.identity);
            _currentItems.Add(item);
            _currentItemsAmount++;
        }
    }
}