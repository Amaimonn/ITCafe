namespace ITCafe
{
    public class PlayerContext
    {
        public IItem CurrentItem { get; set; }
        public IItemPicker ItemPicker { get; set; }

        public PlayerContext(IItemPicker itemPicker)
        {
            ItemPicker = itemPicker;
        }
    }
}