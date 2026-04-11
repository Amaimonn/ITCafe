using System.Collections.Generic;
using DevKit.Utils;
using ITCafe.Data.Items;
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

        private IReadOnlyDictionary<ItemTag, ItemInfoSO> _allItemInfoMap;
        private VisualElement _iconsContainer;
        private Transform _lookAtTarget;
        
        [Inject]
        private void Construct([Key(Constants.ALL_ITEMS_MAP)] IReadOnlyDictionary<ItemTag, ItemInfoSO> allItemInfoMap)
        {
            _allItemInfoMap = allItemInfoMap;
        }
        
        protected override void OnInit()
        {
            _lookAtTarget = Camera.main.transform;

            var root = _uiDocument.rootVisualElement;
            _iconsContainer = root.Q<VisualElement>("ImagesContainer");
            _iconsContainer.Clear();
            
            foreach (var (partTag, amount) in _partsAmountMap)
            {
                if (_allItemInfoMap.TryGetValue(partTag, out var itemInfoSO) && itemInfoSO.Image != null)
                {
                    AddIcons(itemInfoSO.Image, amount);
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