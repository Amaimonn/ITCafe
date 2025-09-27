using ITCafe.CafeBusiness;

namespace ITCafe.Environment
{
    public class BurgerItem : ProductItem, IMenuItem
    {
        public string Id { get; set; }

        private BurgerItemInfo _info;

        public int GetItemHash()
        {
            return _info.GetItemHash();
        }
    }
}