using System.Collections.Generic;
using UnityEngine;

namespace ITCafe.Data.Items
{
    [CreateAssetMenu(fileName = nameof(ItemInfoCollectionSO),
        menuName = "Scriptable Objects/" + nameof(ItemInfoCollectionSO))]
    public class ItemInfoCollectionSO : ScriptableObject, IItemInfoCollection
    {
        public IReadOnlyList<ItemInfoSO> AllInfo => _rawData.AllInfo;

        [SerializeField] private ItemInfoCollection _rawData;
    }
}