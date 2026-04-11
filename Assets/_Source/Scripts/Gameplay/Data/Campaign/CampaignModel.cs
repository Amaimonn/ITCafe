using System.Collections.Generic;
using DevKit.Utils;
using R3;
using ObservableCollections;

namespace ITCafe.Data.Campaign
{
    public class CampaignModel : Model<CampaignState>
    {
        public readonly ReactiveProperty<string> SelectedLocationId;
        public readonly ReactiveProperty<string> SelectedMissionId;
        public readonly ObservableDictionary<string, LocationModel> OpenedLocationsMap;
        public readonly ReactiveProperty<string> LastLaunchedLocationId;
        public readonly ReactiveProperty<string> LastLaunchedMissionId;

        public readonly Subject<Unit> OnStateChanged = new();

        public CampaignModel(CampaignState campaignState) : base(campaignState)
        {
            LastLaunchedLocationId = new ReactiveProperty<string>(campaignState.LastLaunchedLocationId);
            LastLaunchedLocationId.Subscribe(x => campaignState.LastLaunchedLocationId = x);
            
            LastLaunchedMissionId = new ReactiveProperty<string>(campaignState.LastLaunchedMissionId);
            LastLaunchedMissionId.Subscribe(x => campaignState.LastLaunchedMissionId = x);
            
            var selectedLocationId = !string.IsNullOrEmpty(campaignState.SelectedLocationId) ?
                campaignState.SelectedLocationId :
                campaignState.LastLaunchedLocationId;
                
            SelectedLocationId = new ReactiveProperty<string>(selectedLocationId);
            SelectedLocationId.Subscribe(x => campaignState.SelectedLocationId = x);
            
            var selectedMissionId = !string.IsNullOrEmpty(campaignState.SelectedMissionId) ?
                campaignState.SelectedMissionId :
                campaignState.LastLaunchedMissionId;
            
            SelectedMissionId = new ReactiveProperty<string>(selectedMissionId);
            SelectedMissionId.Subscribe(x => campaignState.SelectedMissionId = x);
            
            OpenedLocationsMap = new ObservableDictionary<string, LocationModel>();
            foreach (var location in campaignState.Locations)
            {
                var locationModel = new LocationModel(location);
                if (!OpenedLocationsMap.TryAdd(location.Id, locationModel))
                {
                    FLogger.LogWarning<CampaignModel>($"Failed to add location '{location.Id}' (already exists mb). Overwriting it.");
                    OpenedLocationsMap[location.Id] = locationModel;
                }
            }
            OpenedLocationsMap.ObserveAdd()
                .Subscribe(x => State.Locations.Add(x.Value.Value.State));
        }
    }
}