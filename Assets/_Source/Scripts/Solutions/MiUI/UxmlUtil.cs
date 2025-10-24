using UnityEngine.UIElements;

namespace MiUI
{
    internal static class UxmlUtil
    {
        public static VisualElement CloneStyled(VisualTreeAsset asset)
        {
            var root = asset.CloneTree(); // Создаём UI из uxml, получая при  этом корневой VisualElement для данного View.
            root.pickingMode = PickingMode.Ignore; // По умолчанию корневой элемент не ловит события pointer (можно кликать насквозь).
            root.style.flexGrow = 1;

            return root;
        }

        public static void Show(VisualElement element)
        {
            element.style.display = DisplayStyle.Flex;
        }

        public static void Hide(VisualElement element)
        {
            element.style.display = DisplayStyle.None;
        }
    }
}