using System.Collections.Generic;
using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Data
{
    [CreateAssetMenu(fileName = nameof(RecipeDataSO), menuName = "Scriptable Objects/" + nameof(RecipeDataSO))]
    public class RecipeDataSO : ScriptableObject, IRecipeData
    {
        public IReadOnlyList<ItemTag> RequiredParts => _rawData.RequiredParts;
        public ItemTag CombinationTag =>  _rawData.CombinationTag;
        public ItemTag FinalTag => _rawData.FinalTag;

        [SerializeField] private RecipeData _rawData;
    }
}