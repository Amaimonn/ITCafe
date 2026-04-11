using System.Collections.Generic;
using ITCafe.Data.Items;

namespace ITCafe.Data
{
    public interface IRecipeData
    {
        public IReadOnlyList<ItemTag> RequiredParts { get; }
        public ItemTag CombinationTag { get; }
        public ItemTag FinalTag { get; }
    }
}