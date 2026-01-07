using ITCafe.Data.Settings;
using ITCafe.Data.Campaign;

namespace ITCafe.Infrastructure.Saves
{
    public interface ISaveState
    {
        public SettingsState SettingsState { get; }
        public CampaignState CampaignState { get; }
    }
}