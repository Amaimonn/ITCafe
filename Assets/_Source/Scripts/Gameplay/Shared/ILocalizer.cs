using UnityEngine.UIElements;

namespace ITCafe.Gameplay.Shared
{
    public interface ILocalizer
    {
        public void Localize(Label label, string table, string entry);
        public void Localize(Button button, string table, string entry);
        public void Localize(Tab tab, string table, string entry);
    }
}