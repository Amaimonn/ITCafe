using ITCafe.Player;

namespace ITCafe.Environment
{
    public interface IProcessable
    {
        /// <summary>
        /// Check before calling the <see cref="GetResult"/>
        /// </summary>
        public bool IsProcessable { get; }
        public IItem GetResult(IItem processableItem, PlayerContext context);
    }
}