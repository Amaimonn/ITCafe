using ITCafe.CafeBusiness;

namespace ITCafe.Environment
{
    public interface IItemsContainer : IEquatableItem, IItem
    {
        public bool CanTake();
        public void Take(IMenuItem item); // собирает только готовые блюда, а не ингредиенты
    }
}