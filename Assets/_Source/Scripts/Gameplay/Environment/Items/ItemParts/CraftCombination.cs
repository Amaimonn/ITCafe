using System;
using System.Collections.Generic;
using System.Linq;
using ITCafe.Data.Items;

namespace ITCafe.Environment
{
    /// <summary>
    /// Builder for an item.
    /// </summary>
    public abstract class CraftCombination : ItemPartBase
    {
        public override bool IsCombination => true;
        protected override int ItemHashCode => _itemHashCode;
        
        protected int _itemHashCode;

        /// <summary>
        /// View initialization according to the components present.
        /// </summary>
        public void Init(IEnumerable<ItemTag> itemPartTags) // give it a bunch of SO data (images, sounds etc.) mb
        {
            foreach (var itemPartTag in itemPartTags)
                if (!_partsAmountMap.TryAdd(itemPartTag, 1))
                    _partsAmountMap[itemPartTag]++;
            
            RecalculateItemHash();
            OnInit();
        }
        
        protected abstract void OnInit();

        protected override void RecalculateItemHash()
        {
            var hash = new HashCode();
    
            foreach (var kvp in _partsAmountMap.OrderBy(x => x.Key))
            {
                hash.Add(kvp.Key);
                hash.Add(kvp.Value);
            }
    
            _itemHashCode = hash.ToHashCode();
        }
    }
}