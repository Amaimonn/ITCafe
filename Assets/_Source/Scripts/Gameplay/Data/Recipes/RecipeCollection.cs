using System;
using System.Collections.Generic;
using UnityEngine;

namespace ITCafe.Data
{
    [Serializable]
    public class RecipeCollection : IRecipeCollection
    {
        public IEnumerable<IRecipeData> Recipes => _recipes;

        [field: SerializeField] private RecipeDataSO[] _recipes;
    }
}