using System.Collections.Generic;
using ITCafe.Data.Items;
using ITCafe.Environment;
using ITCafe.Gameplay.Data;
using ITCafe.Player;

namespace ITCafe.CafeBusiness
{
    public class CraftService
    {
        private readonly IEnumerable<RecipeSO> _recipes;
        private readonly IItemsCreator _itemsCreator;

        public CraftService(IEnumerable<RecipeSO> recipes, IItemsCreator itemsCreator)
        {
            _recipes = recipes;
            _itemsCreator = itemsCreator;
        }
        
        public bool TryGetCraft(IItemPart itemPart1, IItemPart itemPart2, out CraftRequest craftRequest)
        {
            craftRequest = default;

            foreach (var recipe in _recipes)
            {
                foreach (var requiredTag in recipe.RequiredParts)
                {
                    if (itemPart1.Tag == ItemTag.Combined)
                    {
                        // TODO: process ItemCombination
                        // check PartsAmountMap
                        // ckeck amount
                    }
                    else
                    {
                        var firstSatisfied = false;
                        var secondSatisfied = false;

                        foreach (var requiredTag2 in recipe.RequiredParts)
                        {
                            if (firstSatisfied && secondSatisfied)
                            {
                                craftRequest = new CraftRequest(itemPart1, itemPart2, recipe);
                                return true;
                            }

                            if (requiredTag == requiredTag2)
                                firstSatisfied = true;
                            else if (requiredTag2 == requiredTag)
                                secondSatisfied = true;
                        }
                    }
                }
            }
            return false;
        }

        public IItem Craft(CraftRequest request)
        {
            var itemPart1 = request.ItemPart1;
            var itemPart2 = request.ItemPart2;
            var recipe = request.Recipe;

            var tags = GetTagsFromParts(itemPart1, itemPart2);
            IItem craftedItem;

            if (tags.Count == recipe.RequiredParts.Length)
            {
                craftedItem = _itemsCreator.Get(recipe.CombinationTag);
            }
            else
            {
                var itemCombination = _itemsCreator.Get<ItemCombination>(recipe.CombinationTag);
                itemCombination.Init(tags);
                craftedItem = itemCombination;
            }

            return craftedItem;
        }

        private List<ItemTag> GetTagsFromParts(IItemPart itemPart1, IItemPart itemPart2)
        {
            var tags = new List<ItemTag>();
            PopulateListFromPart(tags, itemPart1);
            PopulateListFromPart(tags, itemPart2);

            return tags;
        }


        private void PopulateListFromPart(List<ItemTag> list, IItemPart itemPart)
        {
            if (itemPart.Tag == ItemTag.Combined)
            {
                foreach (var (tag, amount) in itemPart.PartsAmountMap)
                    for (var i = 0; i < amount; i++)
                        list.Add(tag);
            }
            else
            {
                list.Add(itemPart.Tag);
            }
        }
    }
}