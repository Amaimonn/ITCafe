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
            return !_isCompleted && context.CurrentItem.CurrentValue is IMenuItem menuItem &&
                   _order.IsCorresponds(menuItem.GetItemHash());
        }

        public override void Interact(PlayerContext context)
        {
            var item = context.CurrentItem.CurrentValue;
            context.ItemPicker.Release();
            Destroy(item.transform.gameObject);
            _isCompleted = true;
        }
    }
}