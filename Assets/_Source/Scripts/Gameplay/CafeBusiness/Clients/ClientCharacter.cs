using System.Linq;
using ITCafe.Environment;
using ITCafe.Gameplay.UI.World;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.CafeBusiness
{
    public class ClientCharacter : BaseInteractable, IItemHandler
    {
        [field: SerializeField] public OrderCloudWorldUI OrderUI { get; private set; }

        private bool IsCompleted => _order.IsCompleted;

        private IOrder _order;

        public void Init(IOrder order)
        {
            _order = order;
        }

#region IInteractable
        public override bool CanInteract(PlayerContext context)
        {
            if (IsCompleted)
                return false;

            var item = context.CurrentItem.CurrentValue;
            if (item != null)
                return item.CanHandle(this, context);

            return false;
        }

        public override void Interact(PlayerContext context)
        {
            var item = context.CurrentItem.CurrentValue;
            item.Handle(this, context);
        }
#endregion

#region IItemHandler
        public bool CanHandle(IItem item, PlayerContext context)
        {
            if (item is IEquatableItem equatableItem)
            {
                var code = equatableItem.GetItemHash();

                if (_order.IsCorresponds(code))
                    return true;
            }

            return false;
        }

        public bool CanHandleContainer(IItemsContainer container, PlayerContext context)
        {
            foreach (var item in container.Items)
            {
                if (_order.IsCorresponds(item.GetItemHash()))
                    return true;
            }

            return false;
        }

        public void Handle(IItem item, PlayerContext context)
        {
            if (item is IEquatableItem equatableItem)
            {
                var hash = equatableItem.GetItemHash();
                if (_order.TryHandOver(hash))
                {
                    context.ItemPicker.Release();
                    ConsumeItem(item, hash);
                }
            }
        }

        public void HandleContainer(IItemsContainer container, PlayerContext context)
        {
            var items = container.Items.ToArray();
            foreach (var it in items)
            {
                if (_order.IsCompleted)
                    break;

                var hash = it.GetItemHash();
                if (_order.IsCorresponds(hash))
                {
                    var item = container.ExtractItem(hash);
                    // Debug.Log($"Extract {hash}");
                    if (item != null && _order.TryHandOver(hash))
                        ConsumeItem(item, hash);
                }
            }
        }
#endregion

        private void ConsumeItem(IItem item, int hash)
        {
            Destroy(item.transform.gameObject);

            if (IsCompleted)
                Destroy(gameObject);
        }
    }
}