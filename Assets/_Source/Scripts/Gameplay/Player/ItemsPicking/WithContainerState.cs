using ITCafe.CafeBusiness;
using ITCafe.Environment;
using UnityEngine;

namespace ITCafe.Player
{
    public class WithContainerState : ItemPickerState
    {
        private readonly IItemsContainer _container;

        public WithContainerState(IItemPicker picker, IItemsContainer container) : base(picker)
        {
            _container = container;
        }

        public override bool CanTake(IItem item)
        {
            return item is IMenuItem menuItem && _container.CanTake(menuItem);
        }

        public override void Take(IItem item)
        {
            if (item is not IMenuItem menuItem)
                return;

            Debug.Log($"Placing item {item.transform.name} on tray");
            _container.Take(menuItem);
        }
    }
}