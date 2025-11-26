using ITCafe.Environment;

namespace ITCafe.Player
{
    public class WithHandlerState : ItemPickerState
    {
        private readonly IItemHandler _handler;

        public WithHandlerState(IItemPicker picker, IItemHandler handler) : base(picker)
        {
            _handler = handler;
        }

        public override bool CanTake(IItem item)
        {
            return item.CanBeHandled(_handler, null);
        }

        public override void Take(IItem item)
        {
            item.BecomeHandled(_handler, null);
        }
    }
}