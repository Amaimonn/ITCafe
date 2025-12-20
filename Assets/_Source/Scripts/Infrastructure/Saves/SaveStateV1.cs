using DevKit.Saves;
using ITCafe.Gameplay.Data;

namespace ITCafe.Infrastructure.Saves
{
    public class SaveStateV1 : SaveStateBase, ISaveState
    {
        public override int Version { get; set; } = 1;
        
        public SettingsState SettingsState { get; set; }
    }
}
