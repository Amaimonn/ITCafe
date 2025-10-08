using ITCafe.Environment;
using UnityEngine;

namespace ITCafe.Player
{
    public class EmptyHandsState : ItemPickerState
    {
        public EmptyHandsState(IItemPicker picker) : base(picker)
        {
        }

        public override bool CanTake(IItem item)
        {
            return item != null;
        }

        public override void Take(IItem item)
        {
            Debug.Log($"Taking item {item.transform.name} with empty hands");
            item.transform.parent = _picker.HoldingPoint;
            item.transform.SetLocalPositionAndRotation(-item.CenterOffset, Quaternion.identity);
            _picker.SetCurrentItem(item);
            
            if (item is IItemHandler handler)
                _picker.ChangeState(new WithHandlerState(_picker, handler));
            // else if (item is IItemsContainer container)
            //     _picker.ChangeState(new WithContainerState(_picker, container));
            else
                _picker.ChangeState(new BusyHandsState(_picker));
            // else if (item is Plate)
            //     _picker.ChangeState(new WithPlateState(_picker, (Plate)item));
        }
    }
}