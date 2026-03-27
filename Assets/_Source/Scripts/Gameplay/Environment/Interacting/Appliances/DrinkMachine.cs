using ITCafe.Data.Items;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment.Appliances
{
    public class DrinkMachine : KitchenAppliance<DrinkMachineProcessable>
    {
        [SerializeField] private ItemTag _fillerTag;

        private IItem _coreItem; // util object

        public override bool CanHandleContainer(IItemsContainer container, PlayerContext context)
        {
            if (_coreItem == null)
            {
                _coreItem = context.ItemsCreator.Get(_fillerTag);
                _coreItem.transform.gameObject.SetActive(false);
                _coreItem.transform.SetParent(transform);
            }

            if (IsBusy && _isReadyResult)
                return context.ItemPicker.CanTake(_holdingItem);
            
            return container.CanTake(_coreItem) && CanHandle(container, context);
        }

        public override void HandleContainer(IItemsContainer container, PlayerContext context)
        {
            Handle(container, context);
        }

        protected override void SetProcessingResult(IProcessableAspect processable, PlayerContext context)
        {
            _holdingItem = processable.GetResult(_holdingItem, context);

            if (_holdingItem is IItemsContainer container)
            {
                var filler = context.ItemsCreator.Get(_fillerTag);
                filler.SetPhysicsEnabled(false);
                processable.AddToResult(container, filler);
            }
        }
    }
}