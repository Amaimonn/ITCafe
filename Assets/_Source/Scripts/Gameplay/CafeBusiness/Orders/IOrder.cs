namespace ITCafe.CafeBusiness
{
    public interface IOrder
    {
        public int OrderHash { get; }

        public bool IsCorresponds(int hash)
        {
            return OrderHash == hash;
        }
    }
}