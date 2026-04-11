using R3;

namespace ITCafe.UI.MVVM
{
    public interface IControlViewModel
    {
        public Observable<bool> OnWarning { get; }
    }
    
    public interface IControlViewModel<TOwn> : IControlViewModel
    {
        public ReadOnlyReactiveProperty<TOwn> OnChanged { get; }
        public void SetValue(TOwn value);
    }
}