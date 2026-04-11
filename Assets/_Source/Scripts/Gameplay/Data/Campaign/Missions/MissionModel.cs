using R3;

namespace ITCafe.Data.Campaign
{
    public class MissionModel : Model<MissionState>
    {
        public ReactiveProperty<bool> IsCompleted { get; }
        public ReactiveProperty<int> Stars { get; }
        public string Id => State.Id;

        public MissionModel(MissionState missionState) : base(missionState)
        {
            IsCompleted = new ReactiveProperty<bool>(State.IsCompleted);
            IsCompleted.Skip(1).Subscribe(x => State.IsCompleted = x);
            
            Stars = new ReactiveProperty<int>(State.Stars);
            Stars.Skip(1).Subscribe(x => State.Stars = x);
        }
    }
}