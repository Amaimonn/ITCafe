using System.Collections.Generic;
using ITCafe.CafeBusiness;

namespace ITCafe.Environment
{
    public interface IItemsContainer : IEquatableItem, IItem
    {
        public IEnumerable<IItem> Items { get; }
        public bool ContainsHash(int hash);
        public IItem ExtractItem(int hash);
        public bool CanTake(IItem item);
        public void Take(IItem item);
    }
}