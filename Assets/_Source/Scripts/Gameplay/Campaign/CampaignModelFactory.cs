using DevKit.Solutions;
using ITCafe.Infrastructure.Saves;

namespace ITCafe.Data.Campaign
{
    public class CampaignModelFactory : IFactory<CampaignModel>
    {
        private readonly ISaveStateProvider _saveStateProvider;

        public CampaignModelFactory(ISaveStateProvider saveStateProvider)
        {
            _saveStateProvider = saveStateProvider;
        }
        
        public CampaignModel Create()
        {
            var campaignModel = new CampaignModel(_saveStateProvider.SaveState.CampaignState);
            
            return campaignModel;
        }
    }
}