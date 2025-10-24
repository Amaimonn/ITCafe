using System;
using UnityEngine.UIElements;

namespace MiUI.MVVM
{
    /// <summary>
    /// Базовый класс для всех виджетов, использующих UI Toolkit для архитектуры MVVM (Model-View-ViewModel).
    /// Виджеты для UI Toolkit имеют функционал представления, но в отличие от View, наследуются от
    /// VisualElement, а не от MonoBehaviour.
    /// </summary>
    [UxmlElement]
    public abstract partial class ToolkitWidget<T> : VisualElement, IDisposable, IView<T> where T : IViewModel
    {
        protected T ViewModel { get; private set; }
        protected VisualElement Root => this;

        public void Bind(T viewModel)
        {
            ViewModel = viewModel;
            OnBind(viewModel);
        }

        /// <summary>
        /// Шаблонный метод для инициализации виджета.
        /// </summary>
        public void Init()
        {
            OnInit();
        }

        /// <summary>
        /// Инициализация виджета.
        /// </summary>
        protected virtual void OnInit()
        {
        }

        protected abstract void OnBind(T viewModel);

        /// <summary>
        /// Показать виджет.
        /// </summary>
        public virtual void Show()
        {
            style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// Скрыть виджет.
        /// </summary>
        public virtual void Hide()
        {
            style.display = DisplayStyle.None;
        }

        public virtual void Dispose()
        {
        }
    }
}