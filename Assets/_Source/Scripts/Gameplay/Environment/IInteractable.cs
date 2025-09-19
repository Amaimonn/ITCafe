namespace ITCafe
{
    public interface IInteractable
    {
        public void Focus();
        public void UnFocus();
        public bool CanInteract(PlayerContext context);
        public void Interact(PlayerContext context);
    }
}