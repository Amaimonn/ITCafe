using ITCafe.Player;

namespace ITCafe.Environment
{
    public interface IProcessable
    {
        public IItem GetResult(IItem processableItem, PlayerContext context);
    }
}