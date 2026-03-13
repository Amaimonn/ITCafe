using UnityEngine;

namespace ITCafe.Data.Items
{
    [CreateAssetMenu(fileName = nameof(ItemInfoSO), menuName = "Scriptable Objects/" + nameof(ItemInfoSO))]
    public class ItemInfoSO : ScriptableObject
    {
        [field: SerializeField] public ItemTag ItemTag { get; private set; }
       
        [field: SerializeField] public string NameLid { get; private set; }
       
        [field: SerializeField] public Sprite Image { get; private set; }
       
        [field: SerializeField] public GameObject Prefab { get; private set; }
        
        [field: SerializeField, Tooltip("For menu items only. Leave empty for other item types.")] 
        public MenuItemExtraSO MenuItemExtra { get; private set; }
    }
}