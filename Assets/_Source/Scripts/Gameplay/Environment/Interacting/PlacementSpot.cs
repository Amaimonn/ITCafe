using ITCafe.CafeBusiness;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public class PlacementSpot : BaseInteractable, IJustItemHandler
    {
        [SerializeField] private Transform _placedTransform;

        private bool IsBusy => _holdingItem != null;
        private IItem _holdingItem;
        private IItemsContainer _holdingContainer;
        
#region IInteractable
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
        
        public override bool CanInteract(PlayerContext context)
        {
            var item = context.OnItemChanged.CurrentValue;

            return item == null ? IsBusy : item.CanBeHandled(this, context);
        }

        public override void Interact(PlayerContext context)
        {
            var item = context.OnItemChanged.CurrentValue;
            if (item == null)
                HandOver(context);
            else
                context.OnItemChanged.CurrentValue.BecomeHandled(this, context);
        }
#endregion

#region IJustItemHandler
        public bool CanHandle(IItem item, PlayerContext context)
        {
            return !IsBusy ||
                   context.ItemPicker.CanTake(_holdingItem) ||
                   CanInteractWithPlacedContainer(item, context);
        }

        public void Handle(IItem item, PlayerContext context)
        {
            if (!IsBusy)
                Place(item, context);
            else if (context.ItemPicker.CanTake(_holdingItem))
                HandOver(context);
            else
                PlaceOnHoldingContainer(item, context);
        }
#endregion

        private bool CanInteractWithPlacedContainer(IItem item, PlayerContext context)
        {
            if (_holdingContainer == null)
                return false;

            return item.TryGetCachedComponent<IMenuAspect>(out var menuItem) && _holdingContainer.CanTake(item);
        }

        private void Place(IItem item, PlayerContext context)
        {
            var itemPicker = context.ItemPicker;

            itemPicker.Release();
            item.transform.SetParent(_placedTransform, worldPositionStays: true);
            item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            _holdingItem = item;
            if (item is IItemsContainer container)
                _holdingContainer = container;

            _holdingItem.Focus();
        }

        private void HandOver(PlayerContext context)
        {
            context.ItemPicker.Take(_holdingItem);
            _holdingItem.UnFocus();
            _holdingItem = null;
            _holdingContainer = null;
        }

        private void PlaceOnHoldingContainer(IItem item, PlayerContext context)
        {
            context.ItemPicker.Release();
            _holdingContainer.Take(item);
            _holdingItem.Focus();
        }
    }
}
