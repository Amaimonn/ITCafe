namespace ITCafe
{
    public class ProductItem : BaseItem
    {
        public override bool CanInteract(PlayerContext context)
        {
            return context.CurrentItem.CurrentValue == null;
        }

        public override void Interact(PlayerContext context)
        {
            SetPhysicsEnabled(false);
            context.ItemPicker.TryTake(this);
        }
    }
}