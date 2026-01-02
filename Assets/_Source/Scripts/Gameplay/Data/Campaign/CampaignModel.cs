using R3;
using ObservableCollections;

namespace ITCafe.Data.Campaign
{
    public class CampaignModel : Model<CampaignState>
    {
        public readonly ReactiveProperty<string> SelectedLocationId;
        public readonly ReactiveProperty<string> SelectedMissionId;
        public readonly ObservableDictionary<string, LocationModel> OpenedLocationsMap;

        public Subject<Unit> OnStateChanged = new();

        public CampaignModel(CampaignState campaignState) : base(campaignState)
        {
            SelectedLocationId = new ReactiveProperty<string>(campaignState.SelectedLocationId);
            SelectedLocationId.Skip(1).Subscribe(x => campaignState.SelectedLocationId = x);
            
            SelectedMissionId = new ReactiveProperty<string>(campaignState.SelectedMissionId);
            SelectedMissionId.Skip(1).Subscribe(x => campaignState.SelectedMissionId = x);
            
            OpenedLocationsMap = new ObservableDictionary<string, LocationModel>();
            foreach (var location in campaignState.Locations)
            {
                var locationModel = new LocationModel(location);
                OpenedLocationsMap.Add(location.Id, locationModel);
            }
            OpenedLocationsMap.ObserveAdd()
                .Subscribe(x => State.Locations.Add(x.Value.Value.State));
        }
    }
}