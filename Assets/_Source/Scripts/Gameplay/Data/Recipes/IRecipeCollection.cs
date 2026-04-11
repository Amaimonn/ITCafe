using System.Collections.Generic;

namespace ITCafe.Data
{
    public interface IRecipeCollection
    {
        public IEnumerable<IRecipeData> Recipes { get; }
    }
}