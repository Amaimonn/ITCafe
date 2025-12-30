using System.Collections.Generic;
using System.Linq;
using R3;
using ObservableCollections;

namespace ITCafe.Data.Campaign
{
    public class LocationModel : Model<LocationState>
    {
        public readonly ReactiveProperty<bool> IsCompleted;
        public readonly ObservableDictionary<string, MissionModel> AvailableMissionsMap;
        public readonly ILocationData Data;

        public LocationModel(LocationState locationState, ILocationData locationData) : 
            base(locationState)
        {
            Data = locationData;
            var availableMissions = locationState.OpenedMissions.Select(x =>
            {
                // TODO: search optimization
                var missionData = locationData.AllMissionsData
                    .FirstOrDefault(m => m.Id == x.Id);
                return new MissionModel(x, missionData);
            });

            IsCompleted = new ReactiveProperty<bool>(locationState.IsCompleted);
            IsCompleted.Skip(1).Subscribe(x => State.IsCompleted = x);
            
            var availableMissionsMap = availableMissions
                .Select(x => new KeyValuePair<string, MissionModel>(x.State.Id, x));
            AvailableMissionsMap = new ObservableDictionary<string, MissionModel>(availableMissionsMap);
            AvailableMissionsMap.ObserveAdd()
                .Subscribe(x => locationState.OpenedMissions.Add(x.Value.Value.State));
        }
    }
}