using ITCafe.Player;

namespace ITCafe.Environment
{
    public interface IProcessableAspect
    {
        /// <summary>
        /// Check before calling the <see cref="GetResult"/>
        /// </summary>
        public bool IsProcessable { get; }
        public float ProcessingTime { get; }
        public IItem GetResult(IItem processableItem, PlayerContext context);
    }
}