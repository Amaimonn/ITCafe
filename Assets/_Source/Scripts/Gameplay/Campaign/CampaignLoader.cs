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
    public class CampaignLoader
    {
        private readonly CampaignModel _campaignModel;
        private readonly Dictionary<string, AsyncOperationHandle> _pathHandleMap = new();
        private MissionDataSO[] _currentMissionsData;
        private const string ALL_LOCATIONS_DATA_PATH = "all_locations_data";

        public CampaignLoader(CampaignModel campaignModel)
        {
            _campaignModel = campaignModel;
        }

        /// <summary>
        /// Use on Campaign model init.
        /// </summary>
        public async UniTaskVoid LoadAllLocationsDataAsync()
        {
            var allLocationsData = await LoadAsync<AllLocationsDataSO>(ALL_LOCATIONS_DATA_PATH);
            _campaignModel.SetAllLocationsData(allLocationsData);
        }

        /// <summary>
        /// Use on location selection in Campaign UI.
        /// </summary>
        public async UniTaskVoid LoadLocationMissionsAsync(string locationId)
        {
            UnloadCurrentMissionsData();

            var locationData = _campaignModel.LocationsDataMap.CurrentValue[locationId];
            var loadedMissions = await LoadManyAsync<MissionDataSO>(locationData.MissionIds); // path is id
            var missionsData = loadedMissions.Where(mission => mission != null)
                .ToArray();

            _campaignModel.CurrentMissionsData.Value = missionsData;
        }

        public void UnloadCurrentMissionsData()
        {
            if (_currentMissionsData == null)
                return;
            
            _campaignModel.SelectedMissionData.Value = null;
            
            foreach (var missionData in _currentMissionsData)
            {
                UnloadAsset(missionData);
                _pathHandleMap.Remove(missionData.Id);
            }
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
        public void UnloadAll()
        {
            _campaignModel.SetAllLocationsData(null);
            _campaignModel.CurrentMissionsData.Value = null;
            _campaignModel.SelectedLocationData.Value = null;
            _campaignModel.SelectedMissionData.Value = null;
            _currentMissionsData = null;

            foreach (var asset in _pathHandleMap.Values)
                ReleaseHandle(asset);

            _pathHandleMap.Clear();
        }

        private async UniTask<T> LoadAsync<T>(string path)
        {
            var handle = Addressables.LoadAssetAsync<T>(path);
            _pathHandleMap.Add(path, handle);

            await handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
                return handle.Result;

            FLogger.LogError<CampaignLoader>($"Failed to load {path}");
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