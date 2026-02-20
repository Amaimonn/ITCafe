using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment.Appliances
{
    public abstract class KitchenAppliance<T> : BaseInteractable, IJustItemHandler where T : IProcessable
    {
        [SerializeField] private Transform _placedTransform;

        private bool IsBusy => _holdingItem != null;
        private IItem _holdingItem;
        private bool _isReadyResult = false;

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
            var emptyHands = item == null;

            if (emptyHands)
                return _isReadyResult && IsBusy; // all fried items can be taken with empty hands (for now)
            else
                return item.CanBeHandled(this, context);
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
            return !IsBusy &&
                   item.TryGetCachedComponent<T>(out var processable) &&
                   processable.IsProcessable ||
                   _isReadyResult && context.ItemPicker.CanTake(_holdingItem);
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
            item.transform.SetParent(_placedTransform, worldPositionStays: true);
            item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            _holdingItem = item;
            _holdingItem.Focus();

            Process(context);
        }

        private void Process(PlayerContext context)
        {
            if (!_holdingItem.TryGetCachedComponent<T>(out var processable) ||
                !processable.IsProcessable)
            {
                return;
            }

            // TODO: add delay

            _holdingItem = processable.GetResult(_holdingItem, context);
            _holdingItem.SetPhysicsEnabled(false);
            _isReadyResult = true;
        }

        private void HandOver(PlayerContext context)
        {
            context.ItemPicker.Take(_holdingItem);
            _holdingItem.UnFocus();
            _holdingItem = null;
            _isReadyResult = false;
        }
    }
}