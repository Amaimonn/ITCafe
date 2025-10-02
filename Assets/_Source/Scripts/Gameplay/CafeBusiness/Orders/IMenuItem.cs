using ITCafe.Environment;

namespace ITCafe.CafeBusiness
{
    public interface IMenuItem : IEquatableItem, IItem
    {
        public string Id { get; }
    }
}