using System;
using ITCafe.CafeBusiness;

namespace ITCafe.Data.Items
{
    public abstract class BaseItemInfo : IEquatableItem
    {
        public abstract int GetItemHash();
    }
    
    public abstract class BaseItemInfo<T> : BaseItemInfo, IEquatable<T>
    {
        public abstract bool Equals(T other);
    }
}