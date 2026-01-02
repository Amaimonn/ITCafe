using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DevKit.Utils;
using ITCafe.Data.Campaign;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ITCafe.Campaign
{
    /// <summary>
    /// Loads and unloads data asynchronously.
    /// Limitations: wait for the data loading operation to be completed before releasing it.
    /// </summary>
    public class CampaignDataLoader
    {
        private readonly Dictionary<string, AsyncOperationHandle> _pathHandleMap = new();
        private MissionDataSO[] _currentMissionsData;

        // /// <summary>
        // /// Use on Campaign model init.
        // /// </summary>
        // public async UniTaskVoid LoadAllLocationsDataAsync(CampaignDataModel campaignDataModel)
        // {
        //     var allLocationsData = await LoadAsync<AllLocationsDataSO>(Constants.ALL_LOCATIONS_DATA_PATH);
        //     campaignDataModel.SetAllLocationsData(allLocationsData);
        // }

        /// <summary>
        /// Use on location selection in Campaign UI.
        /// </summary>
        public async UniTaskVoid SelectLocationAsync(CampaignDataModel campaignDataModel, string locationId)
        {
            if (string.IsNullOrEmpty(locationId))
            {
                campaignDataModel.SelectedLocationData.Value = null;
                UnloadCurrentMissionsData(campaignDataModel);
                return;
            }
            
            if (!campaignDataModel.LocationsDataMap.CurrentValue.TryGetValue(locationId, out var locationData))
            {
                FLogger.LogWarning<CampaignDataLoader>($"No location data found for {locationId}");
                return;
            }
            
            campaignDataModel.SelectedLocationData.Value = locationData;
            UnloadCurrentMissionsData(campaignDataModel);

            var loadedMissions = await LoadManyAsync<MissionDataSO>(locationData.MissionIds); // path is id
            if (loadedMissions != null)
            {
                var missionsData = loadedMissions.Where(mission => mission != null)
                    .ToArray();
                campaignDataModel.SetCurrentMissionsData(missionsData);
            }
            else
            {
                FLogger.LogError<CampaignDataLoader>($"No missions loaded for {locationId}");
            }
        }
        
        // may use additional data in future
        public async UniTaskVoid SelectMissionAsync(CampaignDataModel campaignDataModel, string missionId)
        {
            if (campaignDataModel.SelectedMissionData.CurrentValue != null &&
                campaignDataModel.SelectedMissionData.CurrentValue.Id == missionId)
            {
                return;
            }
            
            if (string.IsNullOrEmpty(missionId))
            {
                campaignDataModel.SelectedMissionData.Value = null;
                return;
            }

            if (campaignDataModel.CurrentMissionsDataMap.CurrentValue == null || 
                !campaignDataModel.CurrentMissionsDataMap.CurrentValue.TryGetValue(missionId, out var missionData))
            {
                FLogger.LogWarning<CampaignDataLoader>($"Can`t receive data for {missionId}");
                return;
            }
            
            campaignDataModel.SelectedMissionData.Value = missionData;
        }

        /// <summary>
        /// Clears currently loaded missions data.
        /// May be used when location selection screen is displayed and no missions visible.
        /// </summary>
        private void UnloadCurrentMissionsData(CampaignDataModel campaignDataModel)
        {
            if (_currentMissionsData == null)
                return;

            campaignDataModel.SelectedMissionData.Value = null;
            FLogger.Log<CampaignDataLoader>("Now SelectedMissionData is null");

            foreach (var missionData in _currentMissionsData)
            {
                UnloadAsset(missionData);
                _pathHandleMap.Remove(missionData.Id);
            }
            FLogger.Log<CampaignDataLoader>($"Unloaded left: {_pathHandleMap.Count}");
            _currentMissionsData = null;
        }

        // public void UnloadAllExceptSelected()
        // {
        //     _campaignModel.SetAllLocationsData(null);
        //     _campaignModel.CurrentMissionsData.Value = null;
        //     _currentMissionsData = null;
        //
        //     var selectedLocationPath = _campaignModel.SelectedLocationData.Value?.Id;
        //     var selectedMissionPath = _campaignModel.SelectedMissionData.Value?.Id;
        //
        //     foreach (var (path, handle) in _pathHandleMap)
        //     {
        //         if (path == selectedLocationPath || path == selectedMissionPath)
        //             continue;
        //
        //         ReleaseHandle(handle);
        //     }
        //
        //     if (!string.IsNullOrEmpty(selectedLocationPath) && !string.IsNullOrEmpty(selectedMissionPath))
        //     {
        //         var hasLocation = _pathHandleMap.TryGetValue(selectedLocationPath, out var selectedLocationAsset);
        //         var hasMission = _pathHandleMap.TryGetValue(selectedMissionPath, out var selectedMissionAsset);
        //
        //         _pathHandleMap.Clear();
        //
        //         if (hasLocation)
        //             _pathHandleMap[selectedLocationPath] = selectedLocationAsset;
        //         if (hasMission)
        //             _pathHandleMap[selectedMissionPath] = selectedMissionAsset;
        //     }
        //     else
        //     {
        //         _pathHandleMap.Clear();
        //     }
        // }

        /// <summary>
        /// Use after Campaign UI closed and returning to the menu.
        /// Use after Campaign UI closed and going to gameplay.
        /// Don`t use data objects after this method (look at selected).
        /// </summary>
        public void UnloadAll(CampaignDataModel campaignDataModel)
        {
            campaignDataModel.SetAllLocationsData(null);
            campaignDataModel.SetCurrentMissionsData(null);
            campaignDataModel.SelectedLocationData.Value = null;
            campaignDataModel.SelectedMissionData.Value = null;
            _currentMissionsData = null;

            foreach (var asset in _pathHandleMap.Values)
                ReleaseHandle(asset);

            _pathHandleMap.Clear();
            FLogger.Log<CampaignDataLoader>($"Unloaded left: {_pathHandleMap.Count}");
        }

        private async UniTask<T> LoadAsync<T>(string path)
        {
            var handle = Addressables.LoadAssetAsync<T>(path);
            _pathHandleMap.Add(path, handle);

            await handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                FLogger.Log<CampaignDataLoader>($"Loaded {handle.Result.ToString()}");
                return handle.Result;
            }

            FLogger.LogError<CampaignDataLoader>($"Failed to load {path}");
            UnloadAsset(handle);
            _pathHandleMap.Remove(path);

            return default;
        }

        private async UniTask<T[]> LoadManyAsync<T>(IEnumerable<string> paths)
        {
            var loadTasks = new List<UniTask<T>>();
            foreach (var path in paths)
            {
                var task = LoadAsync<T>(path);
                loadTasks.Add(task);
            }

            var typedAssets = await UniTask.WhenAll(loadTasks);
            typedAssets = typedAssets.Where(x => !EqualityComparer<T>.Default.Equals(x, default)).ToArray();
            
            return typedAssets;
        }

        private void ReleaseHandle(AsyncOperationHandle handle)
        {
            handle.Release();
        }

        private void UnloadAsset<T>(T asset)
        {
            Addressables.Release(asset);
        }
    }
}