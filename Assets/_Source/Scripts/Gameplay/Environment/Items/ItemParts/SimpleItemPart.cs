using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Environment
{
    public class SimpleItemPart : ItemPartBase
    {
        public override ItemTag Tag => _itemTag;
        
        [SerializeField] private ItemTag _itemTag;
        
        // TEST
        public void SetTag(ItemTag newTag)
        {
            _itemTag = newTag;
        }
    }
}
