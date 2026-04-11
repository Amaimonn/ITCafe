using DevKit.Solutions;
using ITCafe.Infrastructure.Saves;
using R3;

namespace ITCafe.Data.Campaign
{
    public class CampaignModelFactory : IFactory<CampaignModel>
    {
        public ReadOnlyReactiveProperty<CampaignModel> Current => _current;

        private readonly ReactiveProperty<CampaignModel> _current = new();
        private readonly ISaveStateProvider _saveStateProvider;

        public CampaignModelFactory(ISaveStateProvider saveStateProvider)
        {
            _saveStateProvider = saveStateProvider;
        }
        
        public CampaignModel Create()
        {
            var campaignModel = new CampaignModel(_saveStateProvider.SaveState.CampaignState);
            _current.Value = campaignModel;
            
            return campaignModel;
        }
    }
}