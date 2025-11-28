using ITCafe.Environment;
using ITCafe.Gameplay.Data;

namespace ITCafe.CafeBusiness
{
    public interface ICraftService
    {
        public bool TryGetCraft(IItemPart itemPart1, IItemPart itemPart2, out CraftRequest craftRequest);
        public IItem Craft(CraftRequest request);
    }
}