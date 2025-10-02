using ITCafe.Player;

namespace ITCafe.Environment
{
    public class PickUpItem : BaseItem
    {
        public override bool CanInteract(PlayerContext context)
        {
            return context.ItemPicker.CanTake();
        }

        public override void Interact(PlayerContext context)
        {
            SetPhysicsEnabled(false);
            context.ItemPicker.TryTake(this);
        }
    }
}