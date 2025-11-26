using ITCafe.Player;

namespace ITCafe.Environment
{
    public class PattyPart : ItemPartBase
    {
        public override ItemPartTag Tag => ItemPartTag.Patty;

        public override void Handle(IItem item, PlayerContext context)
        {
            var itemPicker = context.ItemPicker;
            itemPicker.Release();
            var burger = context.ItemsCreator.Get<BurgerItem>("burger");
            context.ItemPicker.Take(burger);
            Destroy(item.transform.gameObject);
            Destroy(gameObject);
        }

        public override bool CanBeUsedWith(IItemPart itemPart)
        {
            return itemPart.Tag == ItemPartTag.BurgerBun;
        }
    }
}