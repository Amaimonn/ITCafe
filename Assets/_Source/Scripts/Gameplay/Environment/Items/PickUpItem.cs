using ITCafe.Player;

namespace ITCafe.Environment
{
    public class PickUpItem : BaseItem
    {
        public override bool CanInteract(PlayerContext context)
        {
            return context.ItemPicker.CanTake(this);
        }

        public override void Interact(PlayerContext context)
        {
            SetPhysicsEnabled(false);
            context.ItemPicker.Take(this);
        }
    }
}