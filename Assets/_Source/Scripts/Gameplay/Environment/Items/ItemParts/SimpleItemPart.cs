using DevKit.Utils;
using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Environment
{
    public class SimpleItemPart : ItemPartBase
    {
        public override ItemTag Tag => _itemTag;
        
        [SerializeField] private ItemTag _itemTag;
        
        /// <summary>
        /// Used by UnityEvents for items processing.
        /// </summary>
        public void SetTag(ItemTag newTag)
        {
            _itemTag = newTag;
            FLogger.Log<SimpleItemPart>($"new tag: {newTag}");
        }
    }
}
