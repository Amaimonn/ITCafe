using ITCafe.Environment;

namespace ITCafe.Player
{
    public class BusyHandsState : ItemPickerState
    {
        public BusyHandsState(IItemPicker picker) : base(picker)
        {
        }

        public override bool CanTake(IItem item)
        {
            return false;
        }

        public override void Take(IItem item)
        {
        }
    }
}