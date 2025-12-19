using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Gameplay.Data
{
    [CreateAssetMenu(fileName = "RecipeSO", menuName = "Scriptable Objects/RecipeSO")]
    public class RecipeSO : ScriptableObject
    {
        [field: SerializeField] public ItemTag[] RequiredParts { get; private set; }
        [field: SerializeField] public ItemTag CombinationTag { get; private set; }
        [field: SerializeField] public ItemTag FinalTag { get; private set; }
    }
}
