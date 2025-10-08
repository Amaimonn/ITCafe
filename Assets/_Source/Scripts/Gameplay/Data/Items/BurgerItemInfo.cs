using System;

namespace ITCafe.Data.Items
{
    public class BurgerItemInfo : BaseItemInfo<BurgerItemInfo>
    {
        public bool IsDoubleCheese;
        public bool IsDoublePatty;

        public override bool Equals(BurgerItemInfo other)
        {
            return IsDoubleCheese == other.IsDoubleCheese && IsDoublePatty == other.IsDoublePatty;
        }

        public override int GetItemHash()
        {
            return HashCode.Combine(typeof(BurgerItemInfo), IsDoubleCheese, IsDoublePatty);
        }
    }
}