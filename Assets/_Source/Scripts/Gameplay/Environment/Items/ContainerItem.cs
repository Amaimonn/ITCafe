using System.Collections.Generic;
using ITCafe.CafeBusiness;
using ITCafe.Player;

namespace ITCafe.Environment
{
    public abstract class ContainerItem : PickUpItem, IItemsContainer, IItemHandler
    {
        public abstract IEnumerable<IMenuItem> Items { get; }
        
        public override bool CanHandle(IItemHandler handler, PlayerContext context) =>
            handler.CanHandleContainer(this, context);

        public override void Handle(IItemHandler handler, PlayerContext context) =>
            handler.HandleContainer(this, context);

        public abstract int GetItemHash();

        public abstract bool ContainsHash(int hash);

        public abstract IMenuItem ExtractItem(int hash);

        public abstract bool CanTake(IItem item);

        public abstract void Take(IMenuItem item);
        
        public virtual bool CanHandle(IItem item, PlayerContext context)
        {
            return item is IMenuItem menuItem && CanTake(menuItem);
        }

        public bool CanHandleContainer(IItemsContainer container, PlayerContext context)
        {
            return false;
        }

        public virtual void Handle(IItem item, PlayerContext context)
        {
            if (item is not IMenuItem menuItem)
                return;
            
            Take(menuItem);
        }

        public void HandleContainer(IItemsContainer container, PlayerContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}