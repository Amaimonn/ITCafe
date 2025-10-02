namespace ITCafe.CafeBusiness
{
    public class OrderItem : IOrderItem, IOrder
    {
        public int OrderHash => OrderedItemHash;
        public int OrderedItemHash { get; set; }

        public OrderItem(int orderedItemHash)
        {
            OrderedItemHash = orderedItemHash;
        }
    }
}