using ITCafe.Data;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public abstract class BaseProcessableAspect : MonoBehaviour, IProcessableAspect
    {
        public bool IsProcessable { get; private set; } = true;
        public float ProcessingTime => _processingTime;

        [RealizationSelector(typeof(IProcessingHandler)), SerializeReference] 
        protected IProcessingHandler _handler;
        
        [SerializeField] private float _processingTime = 1f;

        public virtual IItem GetResult(IItem processableItem, PlayerContext context)
        {
            IsProcessable = false; // no more than 1 processing by default
            
            if (_handler == null)
                return processableItem;
            
            return _handler.GetProcessed(processableItem, context);
        }

        public virtual void AddToResult(IItemsContainer resultContainer, IItem additionalResult)
        {
            if (resultContainer.CanTake(additionalResult))
                resultContainer.Take(additionalResult);
        }
    }
}