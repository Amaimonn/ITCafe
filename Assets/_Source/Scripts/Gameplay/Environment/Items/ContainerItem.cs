using System.Collections.Generic;
using ITCafe.CafeBusiness;
using ITCafe.Player;

namespace ITCafe.Environment
{
    public abstract class ContainerItem : PickUpItem, IItemsContainer, IItemHandler
    {
        public abstract IEnumerable<IMenuItem> Items { get; }

#region IItem
        public override bool CanBeHandled(IItemHandler handler, PlayerContext context) =>
            handler.CanHandleContainer(this, context);

        public override void BecomeHandled(IItemHandler handler, PlayerContext context) =>
            handler.HandleContainer(this, context);
#endregion

        public abstract int GetItemHash();

        public abstract bool ContainsHash(int hash);

        public abstract IMenuItem ExtractItem(int hash);

        public abstract bool CanTake(IMenuItem item);

        public abstract void Take(IMenuItem item);

#region IItemsHandler
        public virtual bool CanHandle(IItem item, PlayerContext context)
        {
            // context is null (Picker State)
            return item is IMenuItem menuItem && CanTake(menuItem);
        }

        public bool CanHandleContainer(IItemsContainer container, PlayerContext context)
        {
            // context is null (Picker State)
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
#endregion
    }
}