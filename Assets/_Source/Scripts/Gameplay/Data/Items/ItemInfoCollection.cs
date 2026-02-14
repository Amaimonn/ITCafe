using System;
using System.Collections.Generic;
using UnityEngine;

namespace ITCafe.Data.Items
{
    [Serializable]
    public class ItemInfoCollection : IItemInfoCollection
    {
        public IReadOnlyList<ItemInfoSO> AllInfo =>  _allInfo;
        
        [SerializeField] private ItemInfoSO[] _allInfo;
    }
}