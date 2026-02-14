using System.Collections.Generic;

namespace ITCafe.Data.Items
{
    public interface IItemInfoCollection
    {
        public IReadOnlyList<ItemInfoSO> AllInfo { get; }
    }
}