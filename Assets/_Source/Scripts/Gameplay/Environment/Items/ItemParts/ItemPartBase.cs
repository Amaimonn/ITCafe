using System;
using ITCafe.Player;

namespace ITCafe.Environment
{
    public abstract class ItemPartBase : PickUpItem, IItemPart, IItemHandler
    {
#region IItem
        public override bool CanBeHandled(IItemHandler handler, PlayerContext context) =>
            handler.CanHandle(this, context);

        public override void BecomeHandled(IItemHandler handler, PlayerContext context) =>
            handler.Handle(this, context);
#endregion

#region IItemHandler
        public abstract bool CanHandle(IItem item, PlayerContext context);

        public bool CanHandleContainer(IItemsContainer container, PlayerContext context) => false;

        public abstract void Handle(IItem item, PlayerContext context);

        public void HandleContainer(IItemsContainer container, PlayerContext context) =>
            throw new NotImplementedException();
#endregion

#region IItemPart
        public abstract bool CanBeUsedWith(int itemHash);
#endregion
    }
}