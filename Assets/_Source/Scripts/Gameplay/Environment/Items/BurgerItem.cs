using ITCafe.CafeBusiness;
using ITCafe.Data.Items;

namespace ITCafe.Environment
{
    public class BurgerItem : PickUpItem, IMenuItem
    {
        public string Id { get; set; }

        private BurgerItemInfo _info = new();

        public int GetItemHash()
        {
            return _info.GetItemHash();
        }
    }
}