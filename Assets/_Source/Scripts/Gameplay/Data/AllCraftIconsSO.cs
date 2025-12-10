using System;
using System.Collections.Generic;
using System.Linq;
using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Gameplay.Data
{
    [CreateAssetMenu(fileName = "AllCraftIconsSO", menuName = "Scriptable Objects/AllCraftIconsSO")]
    public class AllCraftIconsSO : ScriptableObject
    {
        public IReadOnlyDictionary<ItemTag, Sprite> CraftIconsMap =>
            _craftIconsMap ??= _craftIcons.ToDictionary(k => k.KeyTag, v => v.Icon);

        public IEnumerable<KeyedIcon> CraftIcons => _craftIcons;
        [SerializeField] private KeyedIcon[] _craftIcons;

        [NonSerialized] private Dictionary<ItemTag, Sprite> _craftIconsMap;
    }

    [Serializable]
    public struct KeyedIcon
    {
        public ItemTag KeyTag;
        public Sprite Icon;
    }
}