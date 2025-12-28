using UnityEngine;

namespace ITCafe.Data.Items
{
    [CreateAssetMenu(fileName = "AllTaggedItemsSO", menuName = "Scriptable Objects/AllTaggedItemsSO")]
    public class AllTaggedItemsSO : ScriptableObject
    {
        [field: SerializeField] public TaggedItemSO[] AllTaggedItems { get; private set; }
    }
}