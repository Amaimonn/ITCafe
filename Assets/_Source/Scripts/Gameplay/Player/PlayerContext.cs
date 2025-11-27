using ITCafe.Environment;
using R3;

namespace ITCafe.Player
{
    public class PlayerContext
    {
        public ReadOnlyReactiveProperty<IItem> CurrentItem { get; }
        public IItemPicker ItemPicker { get; }
        public IItemsCreator ItemsCreator { get; }

        public PlayerContext(IItemPicker itemPicker, IItemsCreator itemsCreator)
        {
            CurrentItem = itemPicker.CurrentItem;
            ItemPicker = itemPicker;
            ItemsCreator = itemsCreator;
        }
    }
}