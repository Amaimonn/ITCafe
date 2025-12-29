using DevKit.Utils;
using ITCafe.Data.Items;
using ITCafe.Gameplay.Data;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace ITCafe.Environment
{
    /// <summary>
    /// Common craft combination implementation.
    /// Reduces the number of unique assets required.
    /// </summary>
    public class SimpleCraftCombination : CraftCombination
    {
        public override ItemTag Tag => ItemTag.SimpleCombination;

        [SerializeField] private Transform _uiHolder;
        [SerializeField] private UIDocument _uiDocument;

        [Inject] private readonly AllCraftIconsSO _craftIconsSO;
        private VisualElement _iconsContainer;
        private Transform _lookAtTarget;
        
        protected override void OnInit()
        {
            _lookAtTarget = Camera.main.transform;

            var root = _uiDocument.rootVisualElement;
            _iconsContainer = root.Q<VisualElement>("ImagesContainer");
            _iconsContainer.Clear();
            
            var iconsMap = _craftIconsSO.CraftIconsMap;
            foreach (var (partTag, amount) in _partsAmountMap)
            {
                if (iconsMap.TryGetValue(partTag, out var icon))
                {
                    AddIcons(icon, amount);
                }
                else
                {
                    FLogger.LogWarning($"No craft icon found for {partTag}");
                    AddIcons(null, amount);
                }
            }
        }

        private void AddIcons(Sprite icon, int amount)
        {
            for (var i = 0; i < amount; i++)
            {
                var iconElement = new VisualElement();
                if (icon != null)
                    iconElement.style.backgroundImage = new StyleBackground(icon);

                iconElement.AddToClassList("order-cloud__item-image");
                _iconsContainer.Add(iconElement);
            }
        }

#region MonoBehaviour
        private void Update()
        {
            _uiHolder.transform.LookAt(_lookAtTarget.position, _lookAtTarget.up);
        }
#endregion
    }
}