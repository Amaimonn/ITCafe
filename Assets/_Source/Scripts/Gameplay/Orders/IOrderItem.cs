namespace ITCafe.Gameplay.Orders
{
    public interface IOrderItem
    {
        public int OrderedItemHash { get; }
    }

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