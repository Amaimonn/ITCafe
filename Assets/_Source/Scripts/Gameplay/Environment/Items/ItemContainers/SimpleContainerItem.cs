using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Environment
{
    public abstract class SimpleContainerItem : ContainerItem, ISimpleContainer
    {
        public ItemTag ContainerTag => _containerTag;
        
        [SerializeField] protected ItemTag _containerTag;
    }
}