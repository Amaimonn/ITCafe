using System.Collections.Generic;
using UnityEngine;

namespace ITCafe.Data
{
    [CreateAssetMenu(fileName = nameof(RecipeCollectionSO),
        menuName = "Scriptable Objects/" + nameof(RecipeCollectionSO))]
    public class RecipeCollectionSO : ScriptableObject, IRecipeCollection
    {
        public IEnumerable<IRecipeData> Recipes => _rawData.Recipes;

        [SerializeField] private RecipeCollection _rawData;
    }
}