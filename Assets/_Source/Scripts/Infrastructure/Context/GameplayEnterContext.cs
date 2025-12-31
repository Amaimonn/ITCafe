namespace ITCafe
{
    /// <summary>
    /// Info to enter Gameplay scene
    /// </summary>
    public class GameplayEnterContext : SceneContext
    {
        public string LocationId { get; set; }
        public string MissionId { get; set; }
        
        public GameplayEnterContext() : base(Scenes.GAMEPLAY)
        {
        }
    }
}