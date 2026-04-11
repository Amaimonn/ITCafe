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
        
        // TODO: Add optional pair (ItemTag MainTag, ItemTag PreferredCombinationTag) for combination specification mb?
        //       if it is MainTag in checking collection then combination with PreferredCombinationTag will be used 

        [SerializeField] private RecipeData _rawData;
    }
}