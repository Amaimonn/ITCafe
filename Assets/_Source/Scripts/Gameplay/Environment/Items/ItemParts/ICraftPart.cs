using System.Collections.Generic;
using ITCafe.CafeBusiness;
using ITCafe.Data.Items;

namespace ITCafe.Environment
{
    public interface ICraftPart : IEquatableItem
    {
        public ItemTag Tag { get; }
        public bool IsCombination { get; }
        public IReadOnlyDictionary<ItemTag, int> PartsAmountMap { get; }
        public bool CanBeUsedWith(ICraftPart craftPart);
    }
}
