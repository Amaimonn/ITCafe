namespace ITCafe.CafeBusiness
{
    public interface IEquatableItem
    {
        public bool IsItemEqual(IEquatableItem other)
        {
            return GetItemHash() == other.GetItemHash();
        }

        public int GetItemHash();
    }
}