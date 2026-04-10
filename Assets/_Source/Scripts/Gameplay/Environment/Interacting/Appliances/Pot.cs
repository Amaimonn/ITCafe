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

            return item.CanBeHandled(this, context);
        }
    }
}