using System.Collections.Generic;
using ITCafe.Data.Items;

namespace ITCafe.Environment
{
    /// <summary>
    /// Builder for an item.
    /// </summary>
    public abstract class ItemCombination : ItemPartBase
    {
        public override ItemTag Tag => ItemTag.Combined;

        /// <summary>
        /// View initialization according to the components present.
        /// </summary>
        public void Init(IEnumerable<ItemTag> itemPartTags)
        {
            foreach (var itemPartTag in itemPartTags)
                if (!_partsAmountMap.TryAdd(itemPartTag, 1))
                    _partsAmountMap[itemPartTag]++;
            
            OnInit();
        }
        protected abstract void OnInit();
    }
}