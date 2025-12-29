using UnityEngine;

namespace ITCafe.Data.Items
{
    [CreateAssetMenu(fileName = "ItemInfoSO", menuName = "Scriptable Objects/ItemInfoSO")]
    public class ItemInfoSO : ScriptableObject
    {
        [field: SerializeField] public ItemTag ItemTag { get; set; }
        
        [field: SerializeField] public Sprite Image { get; private set; }
        
        [field: SerializeField] public GameObject Prefab { get; private set; }
        
        [field: SerializeField, Tooltip("For menu items only. Leave empty for other item types.")] 
        public MenuItemExtraSO MenuItemExtra { get; private set; }
    }
}