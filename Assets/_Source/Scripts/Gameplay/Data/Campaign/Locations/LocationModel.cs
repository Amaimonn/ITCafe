using System.Collections.Generic;
using System.Linq;
using DevKit.Utils;
using R3;
using ObservableCollections;

namespace ITCafe.Data.Campaign
{
    public class LocationModel : Model<LocationState>
    {
        public readonly ReactiveProperty<bool> IsCompleted;
        public readonly ObservableDictionary<string, MissionModel> OpenedMissionsMap;
        
        public LocationModel(LocationState locationState) : 
            base(locationState)
        {
            IsCompleted = new ReactiveProperty<bool>(State.IsCompleted);
            IsCompleted.Skip(1).Subscribe(x => State.IsCompleted = x);
            
            var openedMissionsMap = State.OpenedMissions.Select(x =>
            {
                FLogger.Log<LocationModel>($"{x.Id} is opened");
                return new KeyValuePair<string, MissionModel>(x.Id, new MissionModel(x));
            });
            
            OpenedMissionsMap = new ObservableDictionary<string, MissionModel>(openedMissionsMap);
            OpenedMissionsMap.ObserveAdd()
                .Subscribe(x => State.OpenedMissions.Add(x.Value.Value.State));
        }
    }
}