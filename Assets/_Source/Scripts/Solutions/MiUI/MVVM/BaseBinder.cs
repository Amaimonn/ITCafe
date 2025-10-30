using System;

namespace MiUI.MVVM
{
    /// <summary>
    /// Содержит логику для создания/уничтожения экранов UI (Открыватель/закрыватель UI)
    /// с работой по связыванию View с ViewModel.
    /// </summary>
    /// <typeparam name="TView">Представление (UI)</typeparam>
    /// <typeparam name="TViewModel">Конкретная модель представления, которя наследуется от (реализует) AViewModel</typeparam>
    /// <typeparam name="AViewModel">Абстрактная модель представления, от которой зависит View</typeparam>
    public abstract class BaseBinder<TView, TViewModel, AViewModel> : ILinkEntry
        where TView : IView<AViewModel>
        where AViewModel : IViewModel
        where TViewModel : AViewModel, IDisposable
    {
        protected TView _view;
        protected TViewModel _currentViewModel;

        public BaseBinder(TView view)
        {
            _view = view;
        }

        public abstract void Open();

        public abstract void Close();
    }
}