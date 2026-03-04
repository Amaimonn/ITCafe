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
            return _container.CanTake(item);
        }

        public override void Take(IItem item)
        {
            Debug.Log($"Placing item {item.transform.name} into container");
            _container.Take(item);
        }
    }
}