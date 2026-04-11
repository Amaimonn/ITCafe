using ITCafe.Environment;

namespace ITCafe.Data
{
    public readonly struct CraftRequest
    {
        public readonly ICraftPart CraftPart1;
        public readonly ICraftPart CraftPart2;
        public readonly IRecipeData RecipeData;

        public CraftRequest(ICraftPart craftPart1, ICraftPart craftPart2, IRecipeData recipeData)
        {
            CraftPart1 = craftPart1;
            CraftPart2 = craftPart2;
            RecipeData = recipeData;
        }
    }
}