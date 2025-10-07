using ITCafe.Environment;

namespace ITCafe.Player
{
    public abstract class ItemPickerState
    {
        protected readonly IItemPicker _picker;

        protected ItemPickerState(IItemPicker picker)
        {
            _picker = picker;
        }

        public abstract bool CanTake(IItem item);
        public abstract void Take(IItem item);

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }
    }
}