using System.Collections.Generic;
using System.Linq;
using R3;

namespace ITCafe.Data.Campaign
{
    public class CampaignDataModel
    {
        public ILocationDataCollection LocationDataCollection => _locationDataCollection;
        public Observable<bool> IsLoaded => _isLoaded;

        public ReadOnlyReactiveProperty<IReadOnlyDictionary<string, ILocationData>> LocationsDataMap =>
            _locationsDataMap; // should be loaded before model usage

        public ReadOnlyReactiveProperty<IReadOnlyDictionary<string, IMissionData>> CurrentMissionsDataMap =>
            _currentMissionsDataMap;

        public readonly ReactiveProperty<ILocationData> SelectedLocationData = new();
        public readonly ReactiveProperty<IMissionData> SelectedMissionData = new();

        /// <summary>
        /// All Locations config data in campaign
        /// </summary>
        private ILocationDataCollection _locationDataCollection;
        private readonly ReactiveProperty<IReadOnlyDictionary<string, ILocationData>> _locationsDataMap = new();
        private readonly ReactiveProperty<IReadOnlyDictionary<string, IMissionData>> _currentMissionsDataMap = new();

        private readonly ReactiveProperty<bool> _isLoaded = new(false);

        public void SetAllLocationsData(ILocationDataCollection locationDataCollection)
        {
            _locationDataCollection = locationDataCollection;
            if (_locationDataCollection != null && locationDataCollection.AllData != null)
            {
                var locationsPairs = locationDataCollection.AllData
                    .Select(x => new KeyValuePair<string, ILocationData>(x.Id, x));

                _locationsDataMap.Value = new Dictionary<string, ILocationData>(locationsPairs);
            }
            else
            {
                _locationsDataMap.Value = new Dictionary<string, ILocationData>();
            }

            _isLoaded.Value = true;
        }

        public void SetCurrentMissionsData(IReadOnlyList<IMissionData> currentMissionsData)
        {
            if (currentMissionsData != null)
            {
                var missionPairs = currentMissionsData.Select(x => new KeyValuePair<string, IMissionData>(x.Id, x));
                _currentMissionsDataMap.Value = new Dictionary<string, IMissionData>(missionPairs);
            }
            else
            {
                _currentMissionsDataMap.Value = new Dictionary<string, IMissionData>();
            }
        }
    }
}