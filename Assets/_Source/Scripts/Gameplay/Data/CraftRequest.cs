using ITCafe.Environment;

namespace ITCafe.Gameplay.Data
{
    public readonly struct CraftRequest
    {
        public readonly IItemPart ItemPart1;
        public readonly IItemPart ItemPart2;
        public readonly RecipeSO Recipe;

        public CraftRequest(IItemPart itemPart1, IItemPart itemPart2, RecipeSO recipe)
        {
            ItemPart1 = itemPart1;
            ItemPart2 = itemPart2;
            Recipe = recipe;
        }
    }
}