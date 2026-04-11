using System;
using DevKit.Saves;

namespace ITCafe.Data.Campaign
{
    [Serializable]
    public class MissionState : ICopyable<MissionState>
    {
        public string Id;
        public bool IsCompleted;
        public int Stars;

        public MissionState(string id, bool isCompleted)
        {
            Id = id;
            IsCompleted = isCompleted;
        }

        public MissionState Copy()
        {
            return new MissionState(Id, IsCompleted);
        }
    }
}