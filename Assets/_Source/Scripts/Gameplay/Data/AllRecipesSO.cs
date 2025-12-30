using System.Collections.Generic;
using UnityEngine;

namespace ITCafe.Data
{
    [CreateAssetMenu(fileName = "AllRecipesSO", menuName = "Scriptable Objects/AllRecipesSO")]
    public class AllRecipesSO : ScriptableObject
    {
        public IEnumerable<RecipeSO> Recipes => _recipes;
        
        [field: SerializeField] private RecipeSO[] _recipes;
    }
}