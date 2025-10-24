using UnityEngine.UIElements;

namespace MiUI
{
    /// <summary>
    /// Шаблон для представлений, реализованных через UI Toolkit.
    /// </summary>
    public interface IToolkitView : IView
    {
        public VisualElement InitAndGetRoot();
    }
}