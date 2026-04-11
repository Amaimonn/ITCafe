using ITCafe.Player;

namespace ITCafe.Environment
{
    public class TrashCan : BaseInteractable, IJustItemHandler
    {
#region IInteractable
        public override bool CanInteract(PlayerContext context)
        {
            var item = context.OnItemChanged.CurrentValue;
            return item != null && item.CanBeHandled(this, context); // TODO: Check Item is not important in future
        }

        public override void Interact(PlayerContext context)
        {
            var item = context.OnItemChanged.CurrentValue;
            item.BecomeHandled(this, context);
        }
#endregion

#region IJustItemHandler
        public bool CanHandle(IItem item, PlayerContext context)
        {
            return CheckCanDestroy(item, context);
        }

        public void Handle(IItem item, PlayerContext context)
        {
            DestroyItem(item, context);
        }
#endregion

        private void DestroyItem(IItem item, PlayerContext context)
        {
            context.ItemPicker.Release();
            Destroy(item.transform.gameObject); // TODO: monitor destruction if necessary
        }

        private bool CheckCanDestroy(IItem item, PlayerContext context)
        {
            return true; // TODO: Check if item isn`t important
        }
    }
}
