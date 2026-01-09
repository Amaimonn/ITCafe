using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace ITCafe.Gameplay.Shared
{
    public class UnityLocalizer : ILocalizer
    {
        public void Localize(Label label, string table, string entry)
        {
            label.SetBinding("text", new LocalizedString(table, entry));
        }
        
        public void Localize(Tab tab, string table, string entry)
        {
            tab.SetBinding("label", new LocalizedString(table, entry));
        }
        
        public void Localize(Button button, string table, string entry)
        {
            button.SetBinding("text", new LocalizedString(table, entry));
        }
    }
}