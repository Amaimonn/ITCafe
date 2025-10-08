using System;
using ITCafe.Environment;
using ITCafe.Player;
using R3;
using UnityEngine;

namespace ITCafe
{
    public interface IItemPicker
    {
        public ReadOnlyReactiveProperty<IItem> CurrentItem { get; }
        public Observable<bool> IsHoldingItem { get; }
        public Transform HoldingPoint  { get; }

        public void ChangeState(ItemPickerState newState);
        public void SetCurrentItem(IItem item);
        public bool CanTake(IItem item);
        public void Take(IItem item);
        public bool TryTake(IItem item);
        public void Drop();
        public void Release();
    }
}