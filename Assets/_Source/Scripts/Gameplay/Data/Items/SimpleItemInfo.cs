using System;

namespace ITCafe.Data.Items
{
    [Serializable]
    public class SimpleItemInfo : BaseItemInfo<SimpleItemInfo>
    {
        public ItemTags ItemTag;
        
        public override bool Equals(SimpleItemInfo other)
        {
            return ItemTag == other.ItemTag;
        }

        public override int GetItemHash()
        {
            return HashCode.Combine(typeof(SimpleItemInfo), ItemTag);
        }
    }
}