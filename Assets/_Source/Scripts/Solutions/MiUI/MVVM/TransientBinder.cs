using System;

namespace MiUI.MVVM
{
    /// <summary>
    /// Binder для связывания View с ViewModel, подразумевающий ViewModel Transient (создаётся каждый раз при открытии).
    /// Здесь указывается TViewModel, с которой работает View. Чтобы работать с конкретной ViewModel,
    /// а не с абстрактной (если так указано во View, пр.: View<IViewModel>), используйте класс с расширенными
    /// generic-параметрами (с разделением типов ViewModel на конкретный (который мы хотим передать View) и абстрактный
    /// (который требует View)).
    /// </summary>
    public abstract class TransientBinder<TView, TViewModel> : TransientBinder<TView, TViewModel, TViewModel>
        where TView : IView<TViewModel>
        where TViewModel : IViewModel, IDisposable
    {
        protected TransientBinder(TView view) : base(view)
        {
        }
    }

    /// <summary>
    /// Binder для связывания View с ViewModel, подразумевающий ViewModel Transient (создаётся каждый раз при открытии).
    /// Здесь View может иметь зависимость от абстрактной AViewModel (Пр.: IView<AViewModel>),
    /// при этом в реальности быть связана с конкретной TViewModel, реализующей/наследуемой от
    /// AViewModel (where AViewModel : IViewModel)
    /// </summary>
    public abstract class TransientBinder<TView, TViewModel, AViewModel> : BaseBinder<TView, TViewModel, AViewModel>
        where TView : IView<AViewModel>
        where AViewModel : IViewModel
        where TViewModel : AViewModel, IDisposable
    {
        public TransientBinder(TView view) : base(view)
        {
        }

        protected abstract TViewModel GetViewModel();

        public override void Open()
        {
            _currentViewModel = GetViewModel();
            _view.Bind(_currentViewModel);
            _view.Show();
        }

        public override void Close()
        {
            _view.Hide();
            Disposes.ClearDispose(ref _currentViewModel);
            _view.Dispose();
        }
    }
}