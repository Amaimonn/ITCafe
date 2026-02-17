using System;
using ITCafe.Data.Items;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    [Serializable]
    public class CreateItemProcessing : IProcessingHandler
    {
        [SerializeField] private ItemTag _creationTag;
        [SerializeField] private bool _shouldDestroySourceItem = true;
        
        public IItem GetProcessed(IItem processableItem, PlayerContext context)
        {
            var createdItem = context.ItemsCreator.Get(_creationTag);
            
            if (_shouldDestroySourceItem)
                UnityEngine.Object.Destroy(processableItem.transform.gameObject);

            return createdItem;
        }
    }
}