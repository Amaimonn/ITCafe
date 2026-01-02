using ITCafe.Data.Campaign;
using R3;

namespace ITCafe
{
    /// <summary>
    /// Info to enter Gameplay scene
    /// </summary>
    public class GameplayEnterContext : SceneContext
    {
        public string LocationId { get; set; }
        public string MissionId { get; set; }
        public Subject<CafeMissionResult> CompletionSignal { get; set; }
        
        public GameplayEnterContext() : base(Scenes.GAMEPLAY)
        {
        }
    }
}