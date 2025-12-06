using System.Collections.Generic;
using DevKit.Utils;
using ITCafe.Data.Items;
using ITCafe.Environment;
using ITCafe.Gameplay.Data;
using ITCafe.Player;

namespace ITCafe.CafeBusiness
{
    public class CraftService : ICraftService
    {
        private readonly IEnumerable<RecipeSO> _recipes;
        private readonly IItemsCreator _itemsCreator;

        private ICraftPart _cachedPart1;
        private ICraftPart _cachedPart2;
        private bool _cachedCraftAnswer;
        private CraftRequest _cachedCraftResult;

        public CraftService(IEnumerable<RecipeSO> recipes, IItemsCreator itemsCreator)
        {
            _recipes = recipes;
            _itemsCreator = itemsCreator;
        }

        private bool IsCached(ICraftPart craftPart1, ICraftPart craftPart2)
        {
            if (_cachedPart1 == null || _cachedPart2 == null)
                return false;

            return _cachedPart1.IsItemEqual(craftPart1) && _cachedPart2.IsItemEqual(craftPart2) ||
                   _cachedPart1.IsItemEqual(craftPart2) && _cachedPart2.IsItemEqual(craftPart1);
        }

        private void CacheResult(ICraftPart craftPart1, ICraftPart craftPart2, CraftRequest craftRequest,
            bool isPossible)
        {
            _cachedPart1 = craftPart1;
            _cachedPart2 = craftPart2;
            _cachedCraftResult = craftRequest;
            _cachedCraftAnswer = isPossible;
        }

        private void GetFromCache(out bool isPossible, out CraftRequest craftRequest)
        {
            craftRequest = _cachedCraftResult;
            isPossible = _cachedCraftAnswer;
        }

        public bool TryGetCraft(ICraftPart craftPart1, ICraftPart craftPart2, out CraftRequest craftRequest)
        {
            if (IsCached(craftPart1, craftPart2))
            {
                GetFromCache(out var isPossible, out craftRequest);
                return isPossible;
            }
            
            craftRequest = default;

            if (craftPart1.IsCombination || craftPart2.IsCombination)
            {
                // TODO: process ItemCombination
                // check PartsAmountMap
                // check amount
            }
            else
            {
                foreach (var recipe in _recipes)
                {
                    var firstSatisfied = false;
                    var secondSatisfied = false;

                    foreach (var requiredTag2 in recipe.RequiredParts)
                    {
                        if (firstSatisfied && secondSatisfied)
                            break;

                        if (craftPart1.Tag == requiredTag2 && !firstSatisfied)
                            firstSatisfied = true;
                        else if (craftPart2.Tag == requiredTag2)
                            secondSatisfied = true;
                    }

                    if (firstSatisfied && secondSatisfied)
                    {
                        craftRequest = new CraftRequest(craftPart1, craftPart2, recipe);
                        CacheResult(craftPart1, craftPart2, craftRequest, true);
                        
                        return true;
                    }
                }
            }

            FLogger.Log<CraftService>("No Recipes found");
            CacheResult(craftPart1, craftPart2, craftRequest, false);
            
            return false;
        }

        public IItem Craft(CraftRequest request)
        {
            var itemPart1 = request.CraftPart1;
            var itemPart2 = request.CraftPart2;
            var recipe = request.Recipe;

            var tags = GetTagsFromParts(itemPart1, itemPart2);
            IItem craftedItem;

            if (tags.Count == recipe.RequiredParts.Length)
            {
                craftedItem = _itemsCreator.Get(recipe.FinalTag);
            }
            else
            {
                var itemCombination = _itemsCreator.Get<CraftCombination>(recipe.CombinationTag);
                itemCombination.Init(tags);
                craftedItem = itemCombination;
            }

            return craftedItem;
        }

        private List<ItemTag> GetTagsFromParts(ICraftPart craftPart1, ICraftPart craftPart2)
        {
            var tags = new List<ItemTag>();
            PopulateListFromPart(tags, craftPart1);
            PopulateListFromPart(tags, craftPart2);

            return tags;
        }


        private void PopulateListFromPart(List<ItemTag> list, ICraftPart craftPart)
        {
            if (craftPart.IsCombination)
            {
                foreach (var (tag, amount) in craftPart.PartsAmountMap)
                    for (var i = 0; i < amount; i++)
                        list.Add(tag);
            }
            else
            {
                list.Add(craftPart.Tag);
            }
        }
    }
}