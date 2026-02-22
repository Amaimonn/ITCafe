using ITCafe.CafeBusiness;
using ITCafe.Player;

namespace ITCafe.Environment
{
    public interface IMenuItem : IEquatableItem
    {
        public bool CanBeStored(IItemsContainer container);
    }
}