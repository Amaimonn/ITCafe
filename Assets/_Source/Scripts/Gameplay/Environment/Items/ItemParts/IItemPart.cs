using System.Collections.Generic;

namespace ITCafe.Environment
{
    public enum ItemPartTag
    {
        BurgerBun,
        HotDogBun,
        Patty,
        Cheese,
        Sausage
        // Add modifications mb? (ex.: Hot, Cold, Packed, BunFried, PattyFried)
    }

    public interface IItemPart
    {
        public ItemPartTag Tag { get; }
        public IReadOnlyDictionary<ItemPartTag, int> PartsAmountMap { get; }
        public bool CanBeUsedWith(IItemPart itemPart);
    }
}