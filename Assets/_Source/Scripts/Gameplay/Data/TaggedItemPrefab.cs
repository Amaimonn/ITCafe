using System;
using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Gameplay.Data
{
    [Serializable]
    public struct TaggedItemPrefab
    {
        public ItemTag KeyTag;
        public GameObject GameObject;
    }
}