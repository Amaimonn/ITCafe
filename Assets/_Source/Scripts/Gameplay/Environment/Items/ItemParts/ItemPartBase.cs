using System;
using System.Collections.Generic;
using DevKit.Utils;
using ITCafe.CafeBusiness;
using ITCafe.Data.Items;
using ITCafe.Player;
using VContainer;

namespace ITCafe.Environment
{
    public abstract class ItemPartBase : PickUpItem, ICraftPart, IItemHandler
    {
        public abstract ItemTag Tag { get; }
        public virtual bool IsCombination => false;
        public IReadOnlyDictionary<ItemTag, int> PartsAmountMap => _partsAmountMap;

        protected readonly Dictionary<ItemTag, int> _partsAmountMap = new();
        protected virtual int ItemHashCode => (int)Tag;

#region IItem
        public override bool CanBeHandled(IItemHandler handler, PlayerContext context) =>
            handler.CanHandle(this, context);

        public override void BecomeHandled(IItemHandler handler, PlayerContext context) =>
            handler.Handle(this, context);
#endregion

#region IItemHandler
        public virtual bool CanHandle(IItem item, PlayerContext context)
        {
            return item is ICraftPart itemPart &&
                   context.CraftService.TryGetCraft(itemPart, this, out _) &&
                   itemPart.CanBeUsedWith(this);
        }

        public bool CanHandleContainer(IItemsContainer container, PlayerContext context) => false;

        public virtual void Handle(IItem item, PlayerContext context)
        {
            var craftService = context.CraftService;
            if (!craftService.TryGetCraft((ICraftPart)item, this, out var craftRequest))
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
        public virtual bool CanBeUsedWith(ICraftPart craftPart)
        {
            return true; // TODO: Check in service
        }
#endregion

        /// <summary>
        /// Attention: override this for Combined items to use the tag map instead.
        /// Call after data changes.
        /// </summary>
        protected virtual void RecalculateItemHash()
        {
        }

#region IEquatableItem
        public int GetItemHash()
        {
            return ItemHashCode;
        }
#endregion
    }
}