using R3;

namespace ITCafe
{
    public class PlayerContext
    {
        public ReadOnlyReactiveProperty<IItem> CurrentItem { get; }
        public IItemPicker ItemPicker { get; }

        public PlayerContext(IItemPicker itemPicker)
        {
            CurrentItem = itemPicker.CurrentItem;
            ItemPicker = itemPicker;
        }
    }
}