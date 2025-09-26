namespace ITCafe.Gameplay.Orders
{
    public interface IEquatableItem
    {
        public bool CheckEqual(IEquatableItem other)
        {
            return GetItemHash() ==  other.GetItemHash();
        }
        public int GetItemHash();
    }
}