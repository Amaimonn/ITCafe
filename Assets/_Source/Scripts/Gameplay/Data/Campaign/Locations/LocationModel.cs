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
        
        public LocationModel(LocationState locationState) : 
            base(locationState)
        {
            IsCompleted = new ReactiveProperty<bool>(State.IsCompleted);
            IsCompleted.Skip(1).Subscribe(x => State.IsCompleted = x);
            
            var availableMissionsMap = State.OpenedMissions.Select(x => 
                new KeyValuePair<string, MissionModel>(x.Id, new MissionModel(x)));
            
            AvailableMissionsMap = new ObservableDictionary<string, MissionModel>(availableMissionsMap);
            AvailableMissionsMap.ObserveAdd()
                .Subscribe(x => State.OpenedMissions.Add(x.Value.Value.State));
        }
    }
}