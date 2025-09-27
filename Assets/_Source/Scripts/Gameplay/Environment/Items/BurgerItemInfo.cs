using System;
using ITCafe.Gameplay.Orders;

namespace ITCafe
{
    public struct BurgerItemInfo : IEquatable<BurgerItemInfo>, IEquatableItem
    {
        public bool IsDoubleCheese;
        public bool IsDoublePatty;

        public bool Equals(BurgerItemInfo other)
        {
            return IsDoubleCheese == other.IsDoubleCheese && IsDoublePatty == other.IsDoublePatty;
        }

        public int GetItemHash()
        {
            return HashCode.Combine(typeof(BurgerItemInfo), IsDoubleCheese, IsDoublePatty);
        }
    }
}