using UnityEngine;

namespace ITCafe.Data.Items
{
    [CreateAssetMenu(fileName = "TaggedItemSO", menuName = "Scriptable Objects/TaggedItemSO")]
    public class TaggedItemSO : ScriptableObject
    {
        public ItemTag ItemTag;
        public GameObject Prefab;
    }
}