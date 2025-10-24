using UnityEngine;
using UnityEngine.UIElements;

namespace MiUI.MVVM
{
    /// <summary>
    /// Представление через использование UI Toolkit без привязки к ViewModel.
    /// Используется для сериализации под общей абстракцией.
    /// </summary>
    public abstract class ToolkitView : BaseView, IToolkitView
    {
        /// <summary>
        /// Ассет, содержащий UI для текущего View.
        /// </summary>
        [SerializeField] protected VisualTreeAsset _uiAsset;

        /// <summary>
        /// Доступ к корневому элементу из VisualTreeAsset _uiAsset.
        /// </summary>
        protected VisualElement Root { get; private set; }

        /// <summary>
        /// Публичный метод для инициализации View.
        /// Возвращает корневой элемент Root.
        /// Этот метод необходимо вызывать перед действиями с какими-либо элементами (VisualElement) экрана,
        /// так как до вызова Init() экран вообще не будет существовать.
        /// </summary>
        public VisualElement InitAndGetRoot()
        {
            Init();
            return Root;
        }

        /// <summary>
        /// Базовая инициализация UI.
        /// </summary>
        private void Init()
        {
            Root = UxmlUtil.CloneStyled(_uiAsset);
            OnInit();
        }

        /// <summary>
        /// Показать экран.
        /// </summary>
        public override void Show()
        {
            UxmlUtil.Show(Root);
        }

        /// <summary>
        /// Скрыть экран.
        /// </summary>
        public override void Hide()
        {
            UxmlUtil.Hide(Root);
        }

        /// <summary>
        /// Дополнительная инициализация (вызывается после Init()).
        /// Здесь может осуществляться поиск ссылок на различные VisualElement через Root.Q.
        /// </summary>
        protected virtual void OnInit()
        {
        }
    }

    /// <summary>
    /// Базовый класс для всех View, использующих UI Toolkit для архитектуры MVVM (Model-View-ViewModel).
    /// </summary>
    /// <typeparam name="T">Модель представления с бизнес-логикой для View</typeparam>
    public abstract class ToolkitView<T> : ToolkitView, IView<T> where T : IViewModel
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