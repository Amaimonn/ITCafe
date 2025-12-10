using ITCafe.Environment;
using ITCafe.Gameplay.Data;

namespace ITCafe.CafeBusiness
{
    public class CraftCache
    {
        private ICraftPart _cachedPart1;
        private ICraftPart _cachedPart2;
        private bool _cachedCraftAnswer;
        private CraftRequest _cachedCraftResult;
        
        public bool IsCached(ICraftPart craftPart1, ICraftPart craftPart2)
        {
            if (_cachedPart1 == null || _cachedPart2 == null)
                return false;

            return _cachedPart1.IsItemEqual(craftPart1) && _cachedPart2.IsItemEqual(craftPart2) ||
                   _cachedPart1.IsItemEqual(craftPart2) && _cachedPart2.IsItemEqual(craftPart1);
        }

        public void CacheResult(ICraftPart craftPart1, ICraftPart craftPart2, CraftRequest craftRequest,
            bool isPossible)
        {
            _cachedPart1 = craftPart1;
            _cachedPart2 = craftPart2;
            _cachedCraftResult = craftRequest;
            _cachedCraftAnswer = isPossible;
        }

        public void GetFromCache(out bool isPossible, out CraftRequest craftRequest)
        {
            craftRequest = _cachedCraftResult;
            isPossible = _cachedCraftAnswer;
        }
    }
}