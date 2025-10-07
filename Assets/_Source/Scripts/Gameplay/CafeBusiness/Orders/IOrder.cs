using System.Collections.Generic;
using System.Linq;

namespace ITCafe.CafeBusiness
{
    public interface IOrder
    {
        public bool IsCompleted { get; }
        public IEnumerable<int> OrderHashes { get; }

        public bool IsCorresponds(int hash)
        {
            return OrderHashes.Contains(hash);
        }

        public bool TryHandOver(int hash);
    }
}