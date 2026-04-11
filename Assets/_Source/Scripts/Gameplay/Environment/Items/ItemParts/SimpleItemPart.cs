using DevKit.Utils;
using ITCafe.Data.Items;

namespace ITCafe.Environment
{
    public class SimpleItemPart : ItemPartBase
    {
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
