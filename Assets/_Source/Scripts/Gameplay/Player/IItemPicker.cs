using R3;

namespace ITCafe
{
    public interface IItemPicker
    {
        public ReadOnlyReactiveProperty<IItem> CurrentItem { get; }
        public Observable<bool> IsHoldingItem { get; }
        
        public bool CanTake();
        public void Take(IItem item);
        public bool TryTake(IItem item);
        public void Drop();
        public void Release();
    }
}