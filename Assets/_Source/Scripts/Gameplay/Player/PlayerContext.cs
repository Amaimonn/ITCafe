using ITCafe.CafeBusiness;
using ITCafe.Environment;
using R3;

namespace ITCafe.Player
{
    public class PlayerContext
    {
        public ReadOnlyReactiveProperty<IItem> CurrentItem { get; }
        public IItemPicker ItemPicker { get; }
        public IItemsCreator ItemsCreator { get; }
        public ICraftService  CraftService { get; }

        public PlayerContext(IItemPicker itemPicker, IItemsCreator itemsCreator, ICraftService craftService)
        {
            CurrentItem = itemPicker.CurrentItem;
            ItemPicker = itemPicker;
            ItemsCreator = itemsCreator;
            CraftService = craftService;
        }
    }
}