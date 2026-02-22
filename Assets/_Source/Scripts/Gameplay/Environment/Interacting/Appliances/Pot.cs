using ITCafe.Player;

namespace ITCafe.Environment.Appliances
{
    public class Pot : KitchenAppliance<PotProcessable>
    {
        public override bool CanHandleContainer(IItemsContainer container, PlayerContext context)
        {
            return container.TryGetCachedComponent<IMenuItem>(out var _);
        }
    }
}