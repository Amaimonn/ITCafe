using UnityEngine;

namespace ITCafe.Data.Items
{
    [CreateAssetMenu(fileName = "ItemInfoSO", menuName = "Scriptable Objects/ItemInfoSO")]
    public class ItemInfoSO : ScriptableObject
    {
        [field: SerializeField] public Sprite Image { get; protected set; }

        [ItemTypeSelector, SerializeReference]
        public BaseItemInfo ItemInfo;
    }
}