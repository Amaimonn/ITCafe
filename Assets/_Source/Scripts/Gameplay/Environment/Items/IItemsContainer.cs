using ITCafe.CafeBusiness;

namespace ITCafe.Environment
{
    public interface IItemsContainer : IEquatableItem, IItem
    {
        public bool ContainsHash(int hash);
        public IMenuItem ExtractItem(int hash);
        public bool CanTake(IItem item);
        public void Take(IMenuItem item);
    }
}