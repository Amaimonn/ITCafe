using ITCafe.Player;

namespace ITCafe.Environment.Appliances
{
    public class Pot : KitchenAppliance<PotProcessableAspect>
    {
        public override bool CanInteract(PlayerContext context)
        {
            var item = context.OnItemChanged.CurrentValue;
            var emptyHands = item == null;

            if (emptyHands) // there is no way soup can be taken with empty hands
                return false;

            return _isReadyResult && IsBusy && item.CanBeHandled(this, context);
        }

        public override bool CanHandleContainer(IItemsContainer container, PlayerContext context)
        {
            return IsBusy && _isReadyResult && container.CanTake(_holdingItem);
        }

        public override void HandleContainer(IItemsContainer container, PlayerContext context)
        {
            Handle(container, context);
        }
    }
}