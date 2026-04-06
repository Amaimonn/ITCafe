using UnityEngine.UIElements;

namespace ITCafe.Shared
{
    public interface ILocalizer
    {
        public void Localize(Label label, string table, string entry = null);
        public void Localize(Button button, string table, string entry = null);
        public void Localize(Tab tab, string table, string entry = null);
        public void Localize(DropdownField dropdown, string table, string entry = null);
    }
}