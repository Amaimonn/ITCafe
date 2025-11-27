using ITCafe.Player;

namespace ITCafe.Environment
{
    public class BurgerBunPart : ItemPartBase
    {
        public override ItemPartTag Tag => ItemPartTag.BurgerBun;
        
        public override void Handle(IItem item, PlayerContext context)
        {
            var itemPicker = context.ItemPicker;
            itemPicker.Release();
            var burger = context.ItemsCreator.Get<BurgerItem>(Constants.BURGER);
            context.ItemPicker.Take(burger);
            Destroy(item.transform.gameObject);
            Destroy(gameObject);
        }

        public override bool CanBeUsedWith(IItemPart itemPart)
        {
            return itemPart.Tag == ItemPartTag.Patty;
        }
    }
}