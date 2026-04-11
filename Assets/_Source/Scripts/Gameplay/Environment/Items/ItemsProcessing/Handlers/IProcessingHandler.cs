using ITCafe.Player;

namespace ITCafe.Environment
{
    public interface IProcessingHandler
    {
        public IItem GetProcessed(IItem processableItem, PlayerContext context);
    }
}