using ITCafe.Data;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public abstract class SimpleProcessable : MonoBehaviour, IProcessable
    {
        public bool IsProcessable { get; private set; } = true;
        
        [RealizationSelector(typeof(IProcessingHandler)), SerializeReference] 
        protected IProcessingHandler _handler;

        public virtual IItem GetResult(IItem processableItem, PlayerContext context)
        {
            IsProcessable = false;
            
            return _handler.GetProcessed(processableItem, context);
        }
    }
}