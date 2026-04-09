using System;
using ITCafe.Data;
using ITCafe.Data.Items;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    [Serializable]
    public class SwitchTwoTagProcessing : IProcessingHandler
    {
        [SerializeField] private ItemTag _firstTag;

        [RealizationSelector(typeof(IProcessingHandler)), SerializeReference]
        private IProcessingHandler _firstHandler;

        [SerializeField] private ItemTag _secondTag;

        [RealizationSelector(typeof(IProcessingHandler)), SerializeReference]
        private IProcessingHandler _secondHandler;

        public IItem GetProcessed(IItem processableItem, PlayerContext context)
        {
            if (processableItem.Tag == _firstTag)
                return _firstHandler?.GetProcessed(processableItem, context);
            
            if (processableItem.Tag == _secondTag)
                return _secondHandler?.GetProcessed(processableItem, context);
            
            return null;
        }
    }
}