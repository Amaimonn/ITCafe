using System;
using System.Collections.Generic;
using ITCafe.Player;

namespace ITCafe.Environment
{
    public abstract class ItemPartBase : PickUpItem, IItemPart, IItemHandler
    {
        public abstract ItemPartTag Tag { get; }
        public IReadOnlyDictionary<ItemPartTag, int> PartsAmountMap => _partsAmountMap;
        protected readonly Dictionary<ItemPartTag, int> _partsAmountMap = new();

#region IItem
        public override bool CanBeHandled(IItemHandler handler, PlayerContext context) =>
            handler.CanHandle(this, context);

        public override void BecomeHandled(IItemHandler handler, PlayerContext context) =>
            handler.Handle(this, context);
#endregion

#region IItemHandler
        public virtual bool CanHandle(IItem item, PlayerContext context) =>
            item is IItemPart itemPart && itemPart.CanBeUsedWith(this);

        public bool CanHandleContainer(IItemsContainer container, PlayerContext context) => false;

        public abstract void Handle(IItem item, PlayerContext context);

        public void HandleContainer(IItemsContainer container, PlayerContext context) =>
            throw new NotImplementedException();
#endregion

#region IItemPart
        public abstract bool CanBeUsedWith(IItemPart itemPart);
#endregion
    }
}