using ITCafe.Data;
using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Environment
{
    public class SimpleMenuAspect : MonoBehaviour, IMenuAspect
    {
        [RealizationSelector(typeof(BaseItemInfo)), SerializeReference]
        private BaseItemInfo _info;
        
        [SerializeField] private ItemTag _properContainerTag;

        public int GetItemHash()
        {
            return _info.GetItemHash();
        }

        public bool CanBeStored(IItemsContainer container)
        {
            return container.Tag == _properContainerTag;
        }
    }
}