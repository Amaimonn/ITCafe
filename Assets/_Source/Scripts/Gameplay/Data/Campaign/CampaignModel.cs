using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using R3;
using ObservableCollections;

namespace ITCafe.Data.Campaign
{
    public class CampaignModel : Model<CampaignState>
    {
        public IAllLocationsData AllLocationsData => _allLocationsData;
        
        public readonly ReactiveProperty<ILocationData> SelectedLocation;
        public readonly ReactiveProperty<IMissionData> SelectedMission;
        public readonly ObservableDictionary<string, LocationModel> AvailableLocationsMap;
        public readonly IReadOnlyDictionary<string, ILocationData> LocationsDataMap;
        public readonly ReactiveProperty<ILocationData> LastLaunchedLocation = new();
        public readonly ReactiveProperty<IMissionData> LastLaunchedMission = new();
        public Subject<Unit> OnStateChanged = new();
        
        /// <summary>
        /// All Locations config data in campaign
        /// </summary>
        private readonly IAllLocationsData _allLocationsData;

        public CampaignModel(CampaignState campaignState, IAllLocationsData allLocationsData,
            ILocationData selectedLocationData = null, IMissionData selectedMissionData = null)
            : base(campaignState)
        {
            _allLocationsData = allLocationsData;

            var locationsPairs = allLocationsData.AllData
                .Select(x => new KeyValuePair<string, ILocationData>(x.Id, x));
            LocationsDataMap = new Dictionary<string, ILocationData>(locationsPairs);

            AvailableLocationsMap = new ObservableDictionary<string, LocationModel>();
            foreach (var location in campaignState.Locations)
            {
                if (LocationsDataMap.TryGetValue(location.Id, out var locationData))
                {
                    var locationModel = new LocationModel(location, locationData);
                    AvailableLocationsMap.Add(location.Id, locationModel);
                }
                else
                {
                    Debug.LogWarning($"Location {location.Id} from state not found in locationData map");
                }
            }
            AvailableLocationsMap.ObserveAdd().Subscribe(x => State.Locations.Add(x.Value.Value.State));

            if (!string.IsNullOrEmpty(campaignState.LastLaunchedLocationId))
            {
                // last launched data
                if (LocationsDataMap.TryGetValue(campaignState.LastLaunchedLocationId, out var lastLaunchedLocationData))
                {
                    LastLaunchedLocation.Value = lastLaunchedLocationData;
                    var lastLaunchedMissionData = lastLaunchedLocationData.AllMissionsData
                        .FirstOrDefault(x => x.Id == campaignState.LastLaunchedMissionId);
                    LastLaunchedMission.Value = lastLaunchedMissionData;
                }
            }
            LastLaunchedLocation.Skip(1).Subscribe(x => State.LastLaunchedLocationId = x?.Id);
            LastLaunchedMission.Skip(1).Subscribe(x => State.LastLaunchedMissionId = x?.Id);

            if (selectedLocationData != null)
            {
                // from constructor
                SelectedLocation = new ReactiveProperty<ILocationData>(selectedLocationData);
                SelectedMission = new ReactiveProperty<IMissionData>(selectedMissionData);
            }
            else if (LastLaunchedLocation.Value != null)
            {
                SelectedLocation = new ReactiveProperty<ILocationData>(LastLaunchedLocation.Value);
                if (LastLaunchedMission.Value != null)
                    SelectedMission = new ReactiveProperty<IMissionData>(LastLaunchedMission.Value);
            }
            else
            {
                // default
                var firstLocationData = _allLocationsData.AllData.FirstOrDefault();
                SelectedLocation = new ReactiveProperty<ILocationData>(firstLocationData);
                SelectedMission = new ReactiveProperty<IMissionData>(firstLocationData?.AllMissionsData?
                        .FirstOrDefault());
            }
        }
    }
}