using ITCafe.Player;

namespace ITCafe.Environment
{
    public interface IJustItemHandler : IItemHandler
    {
        bool IItemHandler.CanHandleContainer(IItemsContainer container, PlayerContext context)
        {
            return CanHandle(container, context);
        }
        
        void IItemHandler.HandleContainer(IItemsContainer container, PlayerContext context)
        {
            Handle(container, context);
        }
    }
}