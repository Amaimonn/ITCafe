using System;
using UnityEngine;

namespace ITCafe.Data.Items
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ItemTypeSelectorAttribute : Attribute
    {
    }
    
    [CreateAssetMenu(fileName = "ItemInfoSO", menuName = "Scriptable Objects/ItemInfoSO")]
    public class ItemInfoSO : ScriptableObject
    {
        [field: SerializeField] public Sprite Image { get; protected set; }
        
        [SerializeReference, ItemTypeSelector]
        public BaseItemInfo ItemInfo;
    }
}