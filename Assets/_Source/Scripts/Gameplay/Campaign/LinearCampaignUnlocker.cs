using System;
using System.Collections.Generic;
using System.Linq;
using DevKit.Utils;
using ITCafe.Data.Campaign;
using R3;

namespace ITCafe.Campaign
{
    /// <summary>
    /// Handles missions and locations completion logic.
    /// Limitations: works with a linear campaign (the next available mission/location is determined by its index).
    /// </summary>
    public class LinearCampaignUnlocker
    {
        public Subject<T> CreateMissionCompletionSignal<T>(CampaignModel campaignModel,
            CampaignDataModel campaignDataModel,
            Action saveAction,
            Func<T, CampaignState, bool> successHandler) where T : IMissionResult
        {
            var selectedLocationId = campaignModel.SelectedLocationId.Value;
            var selectedMissionId = campaignModel.SelectedMissionId.Value;
            var campaignState = campaignModel.State;
            var selectedLocationModel = campaignModel.OpenedLocationsMap[selectedLocationId];
            var selectedMissionModel = selectedLocationModel.OpenedMissionsMap[selectedMissionId];

            var actions = new List<Action<T>>();
            var completionSignal = new Subject<T>();

            completionSignal.Subscribe(x =>
            {
                foreach (var action in actions)
                    action?.Invoke(x);
            });

            // Data gathering before unloading (data from Addressables)
            var selectedLocationData = campaignDataModel.SelectedLocationData.Value;
            var currentLocationMissionIds = selectedLocationData.MissionIds;
            var allLocationsData = campaignDataModel.LocationDataCollection.AllData;
            var currentLocationIndex = allLocationsData.IndexWhere(x =>
                x.Id == selectedLocationId);
            var allLocationsDataCount = allLocationsData.Count;

            Action openNextLocation = null;
            if (currentLocationIndex < allLocationsDataCount - 1)
            {
                var nextLocationData = allLocationsData[currentLocationIndex + 1];
                var nextLocationId = nextLocationData.Id;
                var nextMissionId = nextLocationData.MissionIds[0];

                openNextLocation = () =>
                {
                    var existingLocation = campaignState.Locations.FirstOrDefault(x => x.Id == nextLocationId);
                    if (existingLocation == null) // location exists (other version migration)
                    {
                        var firstMissionState = new MissionState(nextMissionId, false);
                        var nextLocationOpenedMissions = new List<MissionState>() { firstMissionState };
                        var nextLocationState = new LocationState(nextLocationId, isCompleted: false,
                            nextLocationOpenedMissions);

                        campaignState.Locations.Add(nextLocationState);
                    }
                    else
                    {
                        var existingFirstMission =
                            existingLocation.OpenedMissions.FirstOrDefault(x => x.Id == nextMissionId);

                        if (existingFirstMission == null) // mission exists (other version migration)
                        {
                            var firstMissionState = new MissionState(nextMissionId, false);
                            existingLocation.OpenedMissions.Add(firstMissionState);
                        }
                    }
                };
            }

            // result handling
            // TIP: don`t use Addressable DATA in lambda, because it will be unloaded that will cause null exception
            actions.Add(result =>
            {
                var isSuccess = successHandler(result, campaignState);
                if (!isSuccess || selectedMissionModel.IsCompleted.Value) // only if NEW mission completion
                    return;

                // New mission completed handling
                selectedMissionModel.IsCompleted.Value = true;

                var currentMissionIndex = currentLocationMissionIds.IndexWhere(x => x == selectedMissionId);

                if (currentMissionIndex < currentLocationMissionIds.Count - 1) // open next mission
                {
                    var nextMissionId = currentLocationMissionIds[currentMissionIndex + 1];
                    
                    // only if it not already opened (other version migration)
                    if (!selectedLocationModel.OpenedMissionsMap.ContainsKey(nextMissionId))
                    {
                        var nextMissionState = new MissionState(nextMissionId, false);
                        var nextMissionModel = new MissionModel(nextMissionState);

                        selectedLocationModel.OpenedMissionsMap.Add(nextMissionId, nextMissionModel);
                    }
                }
                else // open next location (+ mission)
                {
                    selectedLocationModel.IsCompleted.Value = true;

                    if (openNextLocation != null)
                    {
                        openNextLocation();
                    }
                    else // last mission was completed
                    {
                        result.IsGameCompletion = true;
                        FLogger.LogGood<LinearCampaignUnlocker>("ALL MISSIONS COMPLETED");
                    }
                }

                if (string.IsNullOrEmpty(selectedLocationModel.State.MaxCompletedMissionId))
                {
                    selectedLocationModel.State.MaxCompletedMissionId = selectedMissionId;
                }
                else
                {
                    var maxMissionIndex = currentLocationMissionIds.IndexWhere(x =>
                        x == selectedLocationModel.State.MaxCompletedMissionId);

                    if (maxMissionIndex < currentMissionIndex)
                        selectedLocationModel.State.MaxCompletedMissionId = selectedMissionId;
                }
            });

            actions.Add(_ => saveAction());
            return completionSignal;
        }

        /// <summary>
        /// Updates available Locations and Missions according to data.
        /// </summary>
        public void MigrateCampaign(CampaignModel campaignModel, CampaignDataModel campaignDataModel)
        {
            var stateChanged = false;
            var allLocationsData = campaignDataModel.LocationDataCollection.AllData;

            foreach (var locationData in allLocationsData)
            {
                var locationState = campaignModel.State.Locations.FirstOrDefault(x => x.Id == locationData.Id);

                if (locationState != null)
                    continue;

                var previousLocationIndex = allLocationsData.IndexWhere(x => x.Id == locationData.Id) - 1;
                var previousLocationData = allLocationsData.ElementAtOrDefault(previousLocationIndex);

                if (previousLocationData != null)
                {
                    var previousLocationState = campaignModel.State.Locations.FirstOrDefault(x =>
                        x.Id == previousLocationData.Id);
                    if (previousLocationState is { IsCompleted: true })
                    {
                        var firstMissionState = new MissionState(locationData.MissionIds[0], false);
                        var newLocationState = new LocationState(locationData.Id, false,
                            new List<MissionState> { firstMissionState });

                        var newLocationModel = new LocationModel(newLocationState);
                        campaignModel.OpenedLocationsMap.Add(locationData.Id, newLocationModel);

                        stateChanged = true;
                    }
                }
                else if (allLocationsData[0].Id == locationData.Id)
                {
                    var firstMissionState = new MissionState(locationData.MissionIds[0], false);
                    var newLocationState = new LocationState(locationData.Id, false,
                        new List<MissionState> { firstMissionState });

                    var newLocationModel = new LocationModel(newLocationState);
                    campaignModel.OpenedLocationsMap.Add(locationData.Id, newLocationModel);

                    stateChanged = true;
                }
            }

            foreach (var locationState in campaignModel.State.Locations)
            {
                if (string.IsNullOrEmpty(locationState.MaxCompletedMissionId))
                    continue;

                var locationData = campaignDataModel.LocationsDataMap.CurrentValue[locationState.Id];

                var lastCompletedMissionData = locationData.MissionIds.FirstOrDefault(x =>
                    x == locationState.MaxCompletedMissionId);
                int lastCompletedMissionDataIndex;
                if (lastCompletedMissionData == null) // last completed mission data no longer exists
                {
                    lastCompletedMissionDataIndex = locationData.MissionIds.LastIndexWhere(x =>
                        locationState.OpenedMissions.Any(m => m.Id == x));
                }
                else
                {
                    lastCompletedMissionDataIndex = locationData.MissionIds.IndexWhere(x =>
                        x == locationState.MaxCompletedMissionId);
                }

                // open old missions and the new one (if it is necessary)
                var allMissionCount = locationData.MissionIds.Count;
                var lastMissionIndexToOpen = Math.Min(lastCompletedMissionDataIndex + 1, allMissionCount - 1);

                for (var i = 0; i <= lastMissionIndexToOpen; i++)
                {
                    var missionId = locationData.MissionIds[i];
                    var missionState = locationState.OpenedMissions.FirstOrDefault(x => x.Id == missionId);

                    if (missionState == null) // open new mission
                    {
                        var newMissionState = new MissionState(missionId, false);
                        var locationModel = campaignModel.OpenedLocationsMap[locationData.Id];
                        var newMissionModel = new MissionModel(newMissionState);
                        locationModel.OpenedMissionsMap.Add(missionId, newMissionModel);

                        stateChanged = true;
                    }
                }
            }

            if (stateChanged)
                campaignModel.OnStateChanged.OnNext(Unit.Default);
        }
    }
}