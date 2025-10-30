using ITCafe.Player;

namespace ITCafe.Environment
{
    public interface IItemHandler
    {
        public bool CanHandle(IItem item, PlayerContext context);
        public bool CanHandleContainer(IItemsContainer container, PlayerContext context);
        public void Handle(IItem item, PlayerContext context);
        public void HandleContainer(IItemsContainer container, PlayerContext context);
    }
}