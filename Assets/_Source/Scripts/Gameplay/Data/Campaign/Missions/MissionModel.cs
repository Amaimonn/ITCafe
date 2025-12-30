using R3;

namespace ITCafe.Data.Campaign
{
    public class MissionModel : Model<MissionState>
    {
        public ReactiveProperty<bool> IsCompleted { get; }
        public string Id => State.Id;
        public int Stars => State.Stars;

        public MissionModel(MissionState missionState) : base(missionState)
        {
            IsCompleted = new ReactiveProperty<bool>(State.IsCompleted);
            IsCompleted.Skip(1).Subscribe(x => State.IsCompleted = x);
        }
    }
}