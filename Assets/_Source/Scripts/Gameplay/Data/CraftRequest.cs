using ITCafe.Environment;

namespace ITCafe.Gameplay.Data
{
    public readonly struct CraftRequest
    {
        public readonly ICraftPart CraftPart1;
        public readonly ICraftPart CraftPart2;
        public readonly RecipeSO Recipe;

        public CraftRequest(ICraftPart craftPart1, ICraftPart craftPart2, RecipeSO recipe)
        {
            CraftPart1 = craftPart1;
            CraftPart2 = craftPart2;
            Recipe = recipe;
        }
    }
}