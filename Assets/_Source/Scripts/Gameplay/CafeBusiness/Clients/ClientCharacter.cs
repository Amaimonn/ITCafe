using ITCafe.Environment;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.CafeBusiness
{
    public class ClientCharacter : BaseInteractable
    {
        private IOrder _order;
        private bool _isCompleted = false;

        protected override void Awake()
        {
            base.Awake();

            _order = new OrderItem(new BurgerItemInfo()
            {
                IsDoubleCheese = false,
                IsDoublePatty = false,
            }.GetItemHash());
            Debug.Log($"OrderHash: {_order.OrderHash}");
        }

        public override bool CanInteract(PlayerContext context)
        {
            if (_isCompleted)
                return false;
            
            if (context.CurrentItem.CurrentValue is IEquatableItem equatableItem)
            {
                var code = equatableItem.GetItemHash();
                Debug.Log($"item: {code}, order: {_order.OrderHash} ");

                if (_order.IsCorresponds(code))
                    return true;
                else if (context.CurrentItem.CurrentValue is IItemsContainer container)
                    return container.ContainsHash(_order.OrderHash);
            }

            return false;
        }

        public override void Interact(PlayerContext context)
        {
            var item = context.CurrentItem.CurrentValue;

            if (item is IItemsContainer container)
                item = container.ExtractItem(_order.OrderHash);
            else
                context.ItemPicker.Release();

            Destroy(item.transform.gameObject);
            _isCompleted = true;
            Destroy(gameObject);
        }
    }
}