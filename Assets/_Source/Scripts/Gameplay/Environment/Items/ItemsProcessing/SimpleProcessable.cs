using ITCafe.Data;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public abstract class SimpleProcessable : MonoBehaviour, IProcessable
    {
        // [SerializeField] protected ItemTag _newTag;
        // [SerializeField] protected UnityEvent<ItemTag> _onProcessed;
        
        [RealizationSelector(typeof(IProcessingHandler)), SerializeReference] 
        protected IProcessingHandler _handler;
        
        public virtual IItem GetResult(IItem processableItem, PlayerContext context)
        {
            return _handler.GetProcessed(processableItem, context);
        }
    }
}