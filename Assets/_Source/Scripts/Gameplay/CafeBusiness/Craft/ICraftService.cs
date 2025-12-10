using ITCafe.Environment;
using ITCafe.Gameplay.Data;

namespace ITCafe.CafeBusiness
{
    public interface ICraftService
    {
        public bool TryGetCraft(ICraftPart craftPart1, ICraftPart craftPart2, out CraftRequest craftRequest);
        public IItem Craft(CraftRequest request);
    }
}