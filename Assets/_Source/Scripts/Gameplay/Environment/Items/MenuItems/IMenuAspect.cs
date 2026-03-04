using ITCafe.CafeBusiness;

namespace ITCafe.Environment
{
    public interface IMenuAspect : IEquatableItem
    {
        public bool CanBeStored(IItemsContainer container);
    }
}