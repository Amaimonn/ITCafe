using UnityEngine;

namespace ITCafe.Data.Items
{
    [CreateAssetMenu(fileName = "MenuItemExtraSO", menuName = "Scriptable Objects/MenuItemExtraSO")]
    public class MenuItemExtraSO : ScriptableObject
    {
        [ItemTypeSelector, SerializeReference]
        public BaseItemInfo ItemInfo;

        [field: SerializeField, Range(1, 4)] public int ComplexityTimeModifier { get; private set; }
        [field: SerializeField] public int Points { get; private set; } = 10;
    }
}