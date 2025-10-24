using UnityEngine.UIElements;

namespace MiUI.MVVM
{
    /// <summary>
    /// Чистая реализация IToolkitView без наследования от MonoBehaviour.
    /// </summary>
    public class RawVoidToolkitView : IToolkitView
    {
        private readonly VisualTreeAsset _uiAsset;
        private VisualElement _root;

        public RawVoidToolkitView(VisualTreeAsset asset)
        {
            _uiAsset = asset;
        }

        public VisualElement InitAndGetRoot()
        {
            _root = UxmlUtil.CloneStyled(_uiAsset);
            return _root;
        }

        public void Show()
        {
            UxmlUtil.Show(_root);
        }

        public void Hide()
        {
            UxmlUtil.Hide(_root);
        }
    }
}