using System;
using System.Collections.Generic;
using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Data
{
    [Serializable]
    public class RecipeData : IRecipeData
    {
        public IReadOnlyList<ItemTag> RequiredParts => _requiredParts;
        [field: SerializeField] public ItemTag CombinationTag { get; private set; }
        [field: SerializeField] public ItemTag FinalTag { get; private set; }

        [SerializeField] private ItemTag[] _requiredParts;
    }
}