using R3;

namespace ITCafe
{
    public interface IItemPicker
    {
        public ReadOnlyReactiveProperty<IItem> CurrentItem { get; }
        public Observable<bool> IsHoldingItem { get; }

        public void TryPickUp(IItem item);
    }
}