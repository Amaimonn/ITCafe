using ITCafe.Environment;

namespace ITCafe.Player
{
    public class WithHandlerState : ItemPickerState
    {
        private readonly IItemHandler _handler;
        private readonly PlayerContext _playerContext;

        public WithHandlerState(IItemPicker picker, IItemHandler handler, PlayerContext playerContext) : base(picker)
        {
            _handler = handler;
            _playerContext = playerContext;
        }

        public override bool CanTake(IItem item)
        {
            return item.CanBeHandled(_handler, _playerContext);
        }

        public override void Take(IItem item)
        {
            item.BecomeHandled(_handler, _playerContext);
        }
    }
}