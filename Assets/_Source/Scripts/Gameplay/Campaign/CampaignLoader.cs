using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DevKit.Utils;
using ITCafe.Data.Campaign;
using UnityEngine;

namespace ITCafe.Campaign
{
    /// <summary>
    /// Loads and unloads data asynchronously.
    /// Limitations: wait for the data loading operation to be completed before releasing it
    /// </summary>
    public class CampaignLoader
    {
        private readonly CampaignModel _campaignModel;
        private readonly Dictionary<string, Object> _loadedAssetsMap = new();
        private MissionDataSO[] _loadedMissionsData;
        private const string ALL_LOCATIONS_DATA_PATH = "all_locations_data";

        public CampaignLoader(CampaignModel campaignModel)
        {
            _campaignModel = campaignModel;
        }

        public async UniTaskVoid LoadAllLocationsDataAsync()
        {
            var allLocationsData = await LoadAsync<AllLocationsDataSO>(ALL_LOCATIONS_DATA_PATH);
            _campaignModel.SetAllLocationsData(allLocationsData);
        }

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
            if (_loadedMissionsData == null)
                return;

            foreach (var missionData in _loadedMissionsData)
                UnloadAsset(missionData);

            _loadedMissionsData = null;
        }

        public void UnloadUnused()
        {
            var selectedLocationPath = _campaignModel.SelectedLocationData.Value?.Id;
            var selectedMissionPath =  _campaignModel.SelectedMissionData.Value?.Id;
            
            foreach (var (path, asset) in _loadedAssetsMap)
            {
                if (path == selectedLocationPath ||
                    path == selectedMissionPath ||
                    path == ALL_LOCATIONS_DATA_PATH)
                    continue;

                UnloadAsset(asset);
            }

            if (!string.IsNullOrEmpty(selectedLocationPath) && !string.IsNullOrEmpty(selectedMissionPath))
            {
                var hasLocation = _loadedAssetsMap.TryGetValue(selectedLocationPath, out var selectedLocationAsset);
                var hasMission = _loadedAssetsMap.TryGetValue(selectedMissionPath, out var selectedMissionAsset);

                _loadedAssetsMap.Clear();

                if (hasLocation)
                    _loadedAssetsMap[selectedLocationPath] = selectedLocationAsset;
                if (hasMission)
                    _loadedAssetsMap[selectedMissionPath] = selectedMissionAsset;
            }
            else
            {
                _loadedAssetsMap.Clear();
            }

            _campaignModel.CurrentMissionsData.Value = null;
            _loadedMissionsData = null;
        }

        public void UnloadAll()
        {
            foreach (var asset in _loadedAssetsMap.Values)
                UnloadAsset(asset);
            _loadedAssetsMap.Clear();
        }

        private async UniTask<T> LoadAsync<T>(string path) where T : Object
        {
            var asset = await Resources.LoadAsync<T>(path);
            var typedAsset = asset as T;

            if (typedAsset == null)
                FLogger.LogError<CampaignLoader>($"Failed to load {path}");
            else
                _loadedAssetsMap.Add(path, typedAsset);

            return typedAsset;
        }

        private async UniTask<T[]> LoadManyAsync<T>(IEnumerable<string> paths) where T : Object
        {
            var loadTasks = new List<UniTask<T>>();
            foreach (var path in paths)
            {
                var task = LoadAsync<T>(path);
                loadTasks.Add(task);
            }

            var typedAssets = await UniTask.WhenAll(loadTasks);
            typedAssets = typedAssets.Where(mission => mission != null).ToArray();

            return typedAssets;
        }

        private void UnloadAsset(Object asset)
        {
            Resources.UnloadAsset(asset);
        }
    }
}