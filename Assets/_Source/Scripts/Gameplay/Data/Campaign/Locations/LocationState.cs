using System;
using System.Collections.Generic;
using System.Linq;
using DevKit.Saves;

namespace ITCafe.Data.Campaign
{
    [Serializable]
    public class LocationState : ICopyable<LocationState>
    {
        public string Id;
        public bool IsCompleted;
        public List<MissionState> OpenedMissions;
        public string MaxCompletedMissionId;

        public LocationState(string id, bool isCompleted, List<MissionState> openedMissions)
        {
            Id = id;
            IsCompleted = isCompleted;
            OpenedMissions = openedMissions;
        }

        public LocationState Copy()
        {
            var copiedMissions = OpenedMissions?.Select(mission => mission.Copy()).ToList();

            var copy = new LocationState(Id, IsCompleted, copiedMissions)
            {
                MaxCompletedMissionId = MaxCompletedMissionId
            };

            return copy;
        }
    }
}
