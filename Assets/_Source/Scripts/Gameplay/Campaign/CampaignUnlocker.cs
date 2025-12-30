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
    public class CampaignUnlocker
    {
        public Subject<T> CreateMissionCompletionSignal<T>(CampaignModel campaignModel,
            ILocationData selectedLocationData, IMissionData selectedMissionData, Action saveAction, 
            Action<T, CampaignState> resultHandler) where T : IMissionResult
        {
            var locationId = selectedLocationData.Id;
            var missionId = selectedMissionData.Id;
            var currentLocationState = campaignModel.State.Locations.First(x => x.Id == locationId);
            var currentMissionState = currentLocationState.OpenedMissions.First(x => x.Id == missionId);

            var actions = new List<Action<T>>();
            var completionSignal = new Subject<T>();

            if (currentMissionState.IsCompleted)
                return completionSignal;

            completionSignal.Subscribe(x =>
            {
                foreach (var action in actions)
                    action?.Invoke(x);
            });
            actions.Add(x =>
            {
                currentMissionState.IsCompleted = true;
                resultHandler(x, campaignModel.State);
            });
            // TODO: implement results handling
            // if (currentMissionState.Stars != 3)
            //     actions.Add(x =>
            //     {
            //         currentMissionState.IsCompleted = true;
            //         var totalStars = 0;
            //         if (x.FirstStar)
            //             totalStars++;
            //         if (x.SecondStar)
            //             totalStars++;
            //         if (x.ThirdStar)
            //             totalStars++;
            //         currentMissionState.Stars = Mathf.Max(totalStars, currentMissionState.Stars);
            //     }); // mission completed

            if (!currentMissionState.IsCompleted)
            {
                var currentLocationMissionIds = selectedLocationData.MissionIds;
                var currentMissionIndex = currentLocationMissionIds.IndexWhere(x => x == missionId);

                if (currentMissionIndex < currentLocationMissionIds.Count - 1) // open next mission
                {
                    var nextMissionData = currentLocationMissionIds[currentMissionIndex + 1];
                    var nextMissionState = new MissionState(nextMissionData, false);
                    actions.Add(_ =>
                    {
                        campaignModel.State.Locations.First(x => x.Id == locationId).OpenedMissions
                            .Add(nextMissionState);
                    });
                }
                else // open next location (+ mission)
                {
                    actions.Add(_ => currentLocationState.IsCompleted = true);
                    var currentLocationIndex = campaignModel.AllLocationsData.AllData.IndexWhere(x =>
                        x.Id == locationId);
                    if (currentLocationIndex < campaignModel.AllLocationsData.AllData.Count - 1)
                    {
                        var nextLocationData = campaignModel.AllLocationsData.AllData[currentLocationIndex + 1];
                        var firstMissionState = new MissionState(nextLocationData.MissionIds[0], false);
                        var nextLocationOpenedMissions = new List<MissionState>() { firstMissionState };
                        var nextLocationState = new LocationState(nextLocationData.Id, isCompleted: false,
                            nextLocationOpenedMissions);
                        actions.Add(_ => { campaignModel.State.Locations.Add(nextLocationState); });
                    }
                }

                if (string.IsNullOrEmpty(currentLocationState.MaxCompletedMissionId))
                {
                    actions.Add(_ => currentLocationState.MaxCompletedMissionId = missionId);
                }
                else
                {
                    var maxMissionIndex = currentLocationMissionIds.IndexWhere(x =>
                        x == currentLocationState.MaxCompletedMissionId);

                    if (maxMissionIndex < currentMissionIndex)
                        actions.Add(_ => currentLocationState.MaxCompletedMissionId = missionId);
                }
            }

            actions.Add(_ => saveAction());

            return completionSignal;
        }

        /// <summary>
        /// Updates available Locations and Missions according to data.
        /// </summary>
        public void MigrateCampaign(CampaignModel campaignModel)
        {
            var stateChanged = false;
            var allLocationsData = campaignModel.AllLocationsData.AllData;

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
                        campaignModel.AvailableLocationsMap.Add(locationData.Id, newLocationModel);

                        stateChanged = true;
                    }
                }
                else if (allLocationsData[0].Id == locationData.Id)
                {
                    var firstMissionState = new MissionState(locationData.MissionIds[0], false);
                    var newLocationState = new LocationState(locationData.Id, false,
                        new List<MissionState> { firstMissionState });

                    var newLocationModel = new LocationModel(newLocationState);
                    campaignModel.AvailableLocationsMap.Add(locationData.Id, newLocationModel);

                    stateChanged = true;
                }
            }

            foreach (var locationState in campaignModel.State.Locations)
            {
                if (string.IsNullOrEmpty(locationState.MaxCompletedMissionId))
                    continue;
                
                var locationData = campaignModel.LocationsDataMap.CurrentValue[locationState.Id];

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
                        var locationModel = campaignModel.AvailableLocationsMap[locationData.Id];
                        var newMissionModel = new MissionModel(newMissionState);
                        locationModel.AvailableMissionsMap.Add(missionId, newMissionModel);

                        stateChanged = true;
                    }
                }
            }

            if (stateChanged)
                campaignModel.OnStateChanged.OnNext(Unit.Default);
        }
    }
}