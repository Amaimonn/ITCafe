using System.Collections.Generic;
using System.Linq;
using DevKit.Utils;
using ITCafe.Data.Items;
using ITCafe.Environment;
using ITCafe.Data;
using ITCafe.Player;

namespace ITCafe.CafeBusiness
{
    /// <summary>
    /// Current limitations:
    /// 1) 2 different recipes must contain a maximum of 1 common atomic part (otherwise, these parts should form their
    /// own сombined part (with Tag), which should be a priority when crafting).
    /// </summary>
    public class CraftService : ICraftService
    {
        private readonly IEnumerable<RecipeSO> _recipes;
        private readonly IItemsCreator _itemsCreator;
        private readonly Dictionary<RecipeSO, ItemTag[]> _recipeOrderedMap;
        private Dictionary<ItemTag, List<RecipeSO>> _firstEntryRecipeMap;

        private readonly CraftCache _craftCache = new();

        public CraftService(IEnumerable<RecipeSO> recipes, IItemsCreator itemsCreator)
        {
            _recipes = recipes;
            _itemsCreator = itemsCreator;

            _recipeOrderedMap = recipes.ToDictionary(k => k, v => v.RequiredParts.OrderBy(p => p).ToArray());
            BuildFirstEntryRecipeMap();
        }

        public bool TryGetCraft(ICraftPart craftPart1, ICraftPart craftPart2, out CraftRequest craftRequest)
        {
            if (_craftCache.IsCached(craftPart1, craftPart2))
            {
                _craftCache.GetFromCache(out var isPossible, out craftRequest);
                return isPossible;
            }

            craftRequest = default;

            if (craftPart1.IsCombination || craftPart2.IsCombination)
            {
                var orderedTags = GetOrderedTags(craftPart1, craftPart2);
                if (_firstEntryRecipeMap.TryGetValue(orderedTags[0], out var possibleRecipes))
                {
                    foreach (var recipe in possibleRecipes)
                    {
                        if (_recipeOrderedMap.TryGetValue(recipe, out var orderedRecipeTags))
                        {
                            if (orderedTags.Count > orderedRecipeTags.Length)
                                continue;

                            var satisfied = true;
                            for (var i = 0; i < orderedTags.Count; i++)
                            {
                                if (orderedTags[i] != orderedRecipeTags[i])
                                {
                                    satisfied = false;
                                    break;
                                }
                            }

                            if (satisfied)
                            {
                                craftRequest = new CraftRequest(craftPart1, craftPart2, recipe);
                                _craftCache.CacheResult(craftPart1, craftPart2, craftRequest, true);

                                return true;
                            }
                        }
                        else
                        {
                            FLogger.LogError<CraftService>("Recipe tags are not registered in order map");
                        }
                    }
                }
                else
                {
                    FLogger.LogError<CraftService>("Craft part has no recipes registered.");
                }
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
                        _craftCache.CacheResult(craftPart1, craftPart2, craftRequest, true);

                        return true;
                    }
                }
            }

            FLogger.Log<CraftService>("No Recipes found");
            _craftCache.CacheResult(craftPart1, craftPart2, craftRequest, false);

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

        private void BuildFirstEntryRecipeMap()
        {
            _firstEntryRecipeMap = new Dictionary<ItemTag, List<RecipeSO>>();

            foreach (var recipe in _recipes)
            {
                foreach (var tag in recipe.RequiredParts)
                {
                    if (!_firstEntryRecipeMap.ContainsKey(tag))
                        _firstEntryRecipeMap[tag] = new List<RecipeSO> { recipe };
                    else
                        _firstEntryRecipeMap[tag].Add(recipe);
                }
            }
        }

        private List<ItemTag> GetOrderedTags(ICraftPart craftPart1, ICraftPart craftPart2)
        {
            List<ItemTag> orderedTags = new();

            CollectTags(craftPart1, in orderedTags);
            CollectTags(craftPart2, in orderedTags);

            orderedTags.Sort();

            return orderedTags;
        }

        private void CollectTags(ICraftPart craftPart, in List<ItemTag> list)
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