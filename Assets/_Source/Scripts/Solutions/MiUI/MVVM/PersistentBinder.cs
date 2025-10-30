using System;

namespace MiUI.MVVM
{
    /// <summary>
    /// Binder для связывания View с ViewModel, подразумевающий ViewModel Singleton (создающуюся только один раз).
    /// Здесь указывается TViewModel, с которой работает View. Чтобы работать с конкретной ViewModel,
    /// а не с абстрактной (если так указано во View, пр.: View<IViewModel>), используйте класс с расширенными
    /// generic-параметрами (с разделением типов ViewModel на конкретный (который мы хотим передать View) и абстрактный
    /// (который требует View)).
    /// </summary>
    public class PersistentBinder<TView, TViewModel> : PersistentBinder<TView, TViewModel, TViewModel>
        where TView : IView<TViewModel>
        where TViewModel : IViewModel, IDisposable
    {
        public PersistentBinder(TView view, TViewModel viewModel) : base(view, viewModel)
        {
        }
    }

    /// <summary>
    /// Binder для связывания View с ViewModel, подразумевающий ViewModel Singleton (создающуюся только один раз).
    /// Здесь View может иметь зависимость от абстрактной AViewModel (Пр.: IView<AViewModel>),
    /// при этом в реальности быть связана с конкретной TViewModel, реализующей/наследуемой от
    /// AViewModel (where AViewModel : IViewModel)
    /// </summary>
    public class PersistentBinder<TView, TViewModel, AViewModel> : BaseBinder<TView, TViewModel, AViewModel>
        where TView : IView<AViewModel>
        where AViewModel : IViewModel
        where TViewModel : AViewModel, IDisposable
    {
        protected TViewModel _persistentViewModel;

        public PersistentBinder(TView view, TViewModel viewModel) : base(view)
        {
            _persistentViewModel = viewModel;
        }

        public override void Open()
        {
            _view.Bind(_persistentViewModel);
            _view.Show();
        }

        public override void Close()
        {
            _view.Hide();
            _view.Dispose();
        }
    }
}