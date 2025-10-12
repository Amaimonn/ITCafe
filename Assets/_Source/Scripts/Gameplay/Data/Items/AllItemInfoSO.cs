using UnityEngine;

namespace ITCafe.Data.Items
{
    [CreateAssetMenu(fileName = "AllItemInfoSO", menuName = "Scriptable Objects/AllItemInfoSO")]
    public class AllItemInfoSO : ScriptableObject
    {
        [field: SerializeField] public ItemInfoSO[] AllInfo { get; private set; }
    }
}