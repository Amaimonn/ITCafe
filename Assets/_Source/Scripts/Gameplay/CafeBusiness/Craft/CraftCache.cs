using ITCafe.Environment;
using ITCafe.Data;

namespace ITCafe.CafeBusiness
{
    public class CraftCache
    {
        private bool _cachedCraftAnswer;
        private CraftRequest _cachedCraftResult;
        private int? _cachedHash1;
        private int? _cachedHash2;
        
        public bool IsCached(ICraftPart craftPart1, ICraftPart craftPart2)
        {
            if (!_cachedHash1.HasValue || !_cachedHash2.HasValue)
                return false;
            
            var craftHash1 = craftPart1.GetItemHash();
            var craftHash2 = craftPart2.GetItemHash();
            var cachedHash1 = _cachedHash1.Value;
            var cachedHash2 = _cachedHash2.Value;

            return craftHash1 == cachedHash1 && craftHash2 == cachedHash2 ||
                   craftHash1 == cachedHash2 && craftHash2 == cachedHash1;
        }

        public void CacheResult(ICraftPart craftPart1, ICraftPart craftPart2, CraftRequest craftRequest,
            bool isPossible)
        {
            _cachedHash1 = craftPart1.GetHashCode();
            _cachedHash2 = craftPart2.GetHashCode();
            _cachedCraftResult = craftRequest;
            _cachedCraftAnswer = isPossible;
        }

        public void GetFromCache(out bool isPossible, out CraftRequest craftRequest)
        {
            isPossible = _cachedCraftAnswer;
            craftRequest = _cachedCraftResult;
        }
    }
}