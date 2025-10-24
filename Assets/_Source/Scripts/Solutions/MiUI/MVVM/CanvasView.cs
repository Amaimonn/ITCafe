namespace MiUI.MVVM
{
    /// <summary>
    /// Представление для отображения в канвасе без привязки к ViewModel.
    /// Используется для сериализации под общей абстракцией.
    /// </summary>
    public abstract class CanvasView : BaseView
    {
        /// <summary>
        /// Показать экран.
        /// </summary>
        public override void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Скрыть экран.
        /// </summary>
        public override void Hide()
        {
            gameObject.SetActive(false);
        }
    }

    public abstract class CanvasView<T> : CanvasView, IView<T> where T : IViewModel
    {
        protected T ViewModel { get; private set; }

        public void Bind(T viewModel)
        {
            ViewModel = viewModel;
            OnBind(viewModel);
        }

        protected abstract void OnBind(T viewModel);
    }
}