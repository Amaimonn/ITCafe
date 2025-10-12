using System.Collections.Generic;
using System.Linq;

namespace ITCafe.CafeBusiness
{
    public interface IOrder
    {
        public bool IsCompleted { get; }

        public abstract bool IsCorresponds(int hash);

        public bool TryHandOver(int hash);
    }
}