using DevKit.Saves;
using ITCafe.Data.Settings;
using ITCafe.Data.Campaign;

namespace ITCafe.Infrastructure.Saves
{
    public class SaveStateV2 : SaveStateBase, ISaveState
    {
        public override int Version { get; set; } = 2;
        public SettingsState SettingsState { get; set; } 
        public CampaignState CampaignState  { get; set; } 
    }
}