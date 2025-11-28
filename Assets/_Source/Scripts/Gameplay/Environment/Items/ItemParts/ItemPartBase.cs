using System;
using System.Collections.Generic;
using DevKit.Utils;
using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using ITCafe.Player;
using VContainer;

namespace ITCafe.Environment
{
    public abstract class ItemPartBase : PickUpItem, IItemPart, IItemHandler
    {
        public abstract ItemTag Tag { get; }
        public IReadOnlyDictionary<ItemTag, int> PartsAmountMap => _partsAmountMap;

        protected readonly Dictionary<ItemTag, int> _partsAmountMap = new();

#region IItem
        public override bool CanBeHandled(IItemHandler handler, PlayerContext context) =>
            handler.CanHandle(this, context);

        public override void BecomeHandled(IItemHandler handler, PlayerContext context) =>
            handler.Handle(this, context);
#endregion

#region IItemHandler
        public virtual bool CanHandle(IItem item, PlayerContext context)
        {
            return item is IItemPart itemPart &&
                   context.CraftService.TryGetCraft(itemPart, this, out _) &&
                   itemPart.CanBeUsedWith(this);
        }

        public bool CanHandleContainer(IItemsContainer container, PlayerContext context) => false;

        public virtual void Handle(IItem item, PlayerContext context)
        {
            var craftService = context.CraftService;
            if (!craftService.TryGetCraft((IItemPart)item, this, out var craftRequest))
            {
                FLogger.LogWarning<ItemPartBase>("No Recipe in Handle method");
                return;
            }

            var itemPicker = context.ItemPicker;
            itemPicker.Release();
            var craftedItem = craftService.Craft(craftRequest);
            context.ItemPicker.Take(craftedItem);
            Destroy(item.transform.gameObject);
            Destroy(gameObject);
        }

        public void HandleContainer(IItemsContainer container, PlayerContext context) =>
            throw new NotImplementedException();
#endregion

#region IItemPart
        public virtual bool CanBeUsedWith(IItemPart itemPart)
        {
            return true; // TODO: Check in service
        }
#endregion
    }
}