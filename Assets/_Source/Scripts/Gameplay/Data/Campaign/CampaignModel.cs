using System.Collections.Generic;
using System.Linq;
using R3;
using ObservableCollections;

namespace ITCafe.Data.Campaign
{
    public class CampaignModel : Model<CampaignState>
    {
        public IAllLocationsData AllLocationsData => _allLocationsData;
        
        public readonly ReactiveProperty<IReadOnlyList<IMissionData>> CurrentMissionsData = new();
        public readonly ReactiveProperty<LocationModel> SelectedLocation = new();
        public readonly ReactiveProperty<MissionModel> SelectedMission = new();
        public readonly ReactiveProperty<ILocationData> SelectedLocationData = new();
        public readonly ReactiveProperty<IMissionData> SelectedMissionData = new();
        
        public readonly ObservableDictionary<string, LocationModel> AvailableLocationsMap;
        public ReadOnlyReactiveProperty<IReadOnlyDictionary<string, ILocationData>> LocationsDataMap =>
            _locationsDataMap; // should be loaded before model usage
        public Subject<Unit> OnStateChanged = new();

        /// <summary>
        /// All Locations config data in campaign
        /// </summary>
        private IAllLocationsData _allLocationsData;
        private readonly ReactiveProperty<IReadOnlyDictionary<string, ILocationData>> _locationsDataMap = new();

        public CampaignModel(CampaignState campaignState) : base(campaignState)
        {
            AvailableLocationsMap = new ObservableDictionary<string, LocationModel>();
            foreach (var location in campaignState.Locations)
            {
                var locationModel = new LocationModel(location);
                AvailableLocationsMap.Add(location.Id, locationModel);
            }
            AvailableLocationsMap.ObserveAdd().Subscribe(x => State.Locations.Add(x.Value.Value.State));
        }

        public void SetAllLocationsData(IAllLocationsData allLocationsData)
        {
            _allLocationsData = allLocationsData;
            if (_allLocationsData != null)
            {
                var locationsPairs = allLocationsData?.AllData
                    .Select(x => new KeyValuePair<string, ILocationData>(x.Id, x));
            
                _locationsDataMap.Value = new Dictionary<string, ILocationData>(locationsPairs);
            }
            else
            {
                _locationsDataMap.Value = new Dictionary<string, ILocationData>();
            }
        }
    }
}