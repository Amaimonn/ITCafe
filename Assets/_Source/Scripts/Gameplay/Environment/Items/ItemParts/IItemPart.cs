using System.Collections.Generic;
using ITCafe.Data.Items;

namespace ITCafe.Environment
{
    public interface IItemPart
    {
        // Add modifications mb? (ex.: Hot, Cold, Packed, BunFried, PattyFried)
        public ItemTag Tag { get; }
        public IReadOnlyDictionary<ItemTag, int> PartsAmountMap { get; }
        public bool CanBeUsedWith(IItemPart itemPart);
    }
}