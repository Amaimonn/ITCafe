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
            IsCompleted.Subscribe(x => State.IsCompleted = x);
            
            OpenedMissionsMap = new ObservableDictionary<string, MissionModel>();

            if (State.OpenedMissions != null)
            {
                foreach (var openedMission in State.OpenedMissions)
                {
                    var missionModel = new MissionModel(openedMission);
                    if (!OpenedMissionsMap.TryAdd(openedMission.Id, missionModel))
                    {
                        FLogger.LogWarning<LocationModel>(
                            $"Failed to add mission '{openedMission.Id}' (already exists mb). Overwriting it.");
                        OpenedMissionsMap[openedMission.Id] = missionModel;
                    }
                }
            }

            OpenedMissionsMap.ObserveAdd()
                .Subscribe(x => State.OpenedMissions.Add(x.Value.Value.State));
        }
    }
}