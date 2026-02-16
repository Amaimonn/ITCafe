using System;
using ITCafe.Data.Items;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public interface IProcessingHandler
    {
        public IItem GetProcessed(IItem processableItem, PlayerContext context);
    }
    
    [Serializable]
    public class CreateItemProcessing : IProcessingHandler
    {
        [SerializeField] protected ItemTag _creationTag;
        [SerializeField] protected bool _shouldDestroySourceItem = true;
        
        public IItem GetProcessed(IItem processableItem, PlayerContext context)
        {
            var createdItem = context.ItemsCreator.Get(_creationTag);
            
            if (_shouldDestroySourceItem)
                UnityEngine.Object.Destroy(processableItem.transform.gameObject);

            return createdItem;
        }
    }
}