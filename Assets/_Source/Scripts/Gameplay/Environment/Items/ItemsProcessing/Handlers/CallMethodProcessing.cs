using System;
using ITCafe.Data.Items;
using ITCafe.Player;
using UnityEngine;
using UnityEngine.Events;

namespace ITCafe.Environment
{
    [Serializable]
    public class CallMethodProcessing : IProcessingHandler
    {
        [SerializeField] private ItemTag _resultTag;
        [SerializeField] private UnityEvent<ItemTag> _onTagChanged;
        [SerializeField] private UnityEvent _onProcessed;
        
        public IItem GetProcessed(IItem processableItem, PlayerContext context)
        {
            _onTagChanged?.Invoke(_resultTag);
            _onProcessed?.Invoke();
            
            return processableItem;
        }
    }
}