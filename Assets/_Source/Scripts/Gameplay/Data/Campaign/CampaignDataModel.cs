using System.Collections.Generic;
using System.Linq;
using R3;

namespace ITCafe.Data.Campaign
{
    public class CampaignDataModel
    {
        public IAllLocationsData AllLocationsData => _allLocationsData;
        public Observable<bool> IsLoaded => _isLoaded;

        public ReadOnlyReactiveProperty<IReadOnlyDictionary<string, ILocationData>> LocationsDataMap =>
            _locationsDataMap; // should be loaded before model usage

        public readonly ReactiveProperty<ILocationData> SelectedLocationData = new();
        public readonly ReactiveProperty<IMissionData> SelectedMissionData = new();
        public readonly ReactiveProperty<IReadOnlyList<IMissionData>> CurrentMissionsData = new();

        /// <summary>
        /// All Locations config data in campaign
        /// </summary>
        private IAllLocationsData _allLocationsData;
        private readonly ReactiveProperty<IReadOnlyDictionary<string, ILocationData>> _locationsDataMap = new();

        private readonly ReactiveProperty<bool> _isLoaded = new(false);

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
            
            _isLoaded.Value = true;
        }
    }
}