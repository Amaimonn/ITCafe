using System.Collections.Generic;
using ITCafe.Player;

namespace ITCafe.Environment
{
    public abstract class ContainerItem : PickUpItem, IItemsContainer, IItemHandler
    {
        public abstract IEnumerable<IItem> Items { get; }

#region IItem
        public override bool CanBeHandled(IItemHandler handler, PlayerContext context) =>
            handler.CanHandleContainer(this, context);

        public override void BecomeHandled(IItemHandler handler, PlayerContext context) =>
            handler.HandleContainer(this, context);
#endregion

        public override void Focus()
        {
            base.Focus();
            if (Items != null)
                foreach (var item in Items)
                    item?.Focus();
        }

        public override void UnFocus()
        {
            base.UnFocus();
            if (Items != null)
                foreach (var item in Items)
                    item?.UnFocus();
        }

#region IEquatableItem
        public abstract int GetItemHash();
#endregion

#region IItemsContainer
        public abstract bool ContainsHash(int hash);

        public abstract IItem ExtractItem(int hash);

        public abstract bool CanTake(IItem item); // there could be recipe check

        public abstract void Take(IItem item); // there could be crafting
#endregion

#region IItemsHandler
        public virtual bool CanHandle(IItem item, PlayerContext context)
        {
            // context is null (Picker State)
            return CanTake(item);
        }

        public virtual bool CanHandleContainer(IItemsContainer container, PlayerContext context)
        {
            return false;
        }

        public virtual void Handle(IItem item, PlayerContext context)
        {
            Take(item);
        }

        public virtual void HandleContainer(IItemsContainer container, PlayerContext context)
        {
            throw new System.NotImplementedException();
        }
#endregion
    }
}