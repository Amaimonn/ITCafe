using DevKit.Utils;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public class PlacementSpot : BaseInteractable, IJustItemHandler
    {
        [SerializeField] private Transform _placedTransform;

        private bool IsBusy => _holdingItem != null;
        private IItem _holdingItem;

        public override void Focus()
        {
            base.Focus();
            _holdingItem?.Focus();
        }
        
        public override void UnFocus()
        {
            base.UnFocus();
            _holdingItem?.UnFocus();
        }

#region IInteractable
        public override bool CanInteract(PlayerContext context)
        {
            var item = context.CurrentItem.CurrentValue;

            return item == null ? IsBusy : item.CanBeHandled(this, context);
        }

        public override void Interact(PlayerContext context)
        {
            var item = context.CurrentItem.CurrentValue;
            if (item == null)
                HandOver(context);
            else
                context.CurrentItem.CurrentValue.BecomeHandled(this, context);
        }
#endregion

#region IJustItemHandler
        public bool CanHandle(IItem item, PlayerContext context)
        {
            return !IsBusy || context.ItemPicker.CanTake(_holdingItem);
        }

        public void Handle(IItem item, PlayerContext context)
        {
            if (!IsBusy)
                Place(item, context);
            else
                HandOver(context);
        }
#endregion

        private void Place(IItem item, PlayerContext context)
        {
            var itemPicker = context.ItemPicker;

            itemPicker.Release();
            item.transform.SetParent(_placedTransform, false);
            item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            _holdingItem = item;
            _holdingItem.Focus();
        }

        private void HandOver(PlayerContext context)
        {
            context.ItemPicker.Take(_holdingItem);
            _holdingItem.UnFocus();
            _holdingItem = null;
        }
    }
}