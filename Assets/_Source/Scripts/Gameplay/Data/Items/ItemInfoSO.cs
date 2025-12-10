using UnityEngine;

namespace ITCafe.Data.Items
{
    [CreateAssetMenu(fileName = "ItemInfoSO", menuName = "Scriptable Objects/ItemInfoSO")]
    public class ItemInfoSO : ScriptableObject
    {
        [field: SerializeField, Range(1, 4)] public int ComplexityTimeModifier { get; private set; }
        [field: SerializeField] public Sprite Image { get; private set; }

        [ItemTypeSelector, SerializeReference]
        public BaseItemInfo ItemInfo;

        [field: SerializeField] public int Points { get; private set; } = 10;
    }
}