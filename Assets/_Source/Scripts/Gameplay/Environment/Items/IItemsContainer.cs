using System.Collections;
using System.Collections.Generic;
using ITCafe.CafeBusiness;

namespace ITCafe.Environment
{
    public interface IItemsContainer : IEquatableItem, IItem
    {
        public IEnumerable<IMenuItem> Items { get; }
        public bool ContainsHash(int hash);
        public IMenuItem ExtractItem(int hash);
        public bool CanTake(IMenuItem item);
        public void Take(IMenuItem item);
    }
}