
namespace ITCafe
{
    /// <summary>
    /// Info to get back to the MainMenu from the Gameplay scene. Exiting the Gameplay leads to the MainMenu.
    /// </summary>
    public class GameplayExitContext
    {
        public SceneContext EnterContext { get; }

        public GameplayExitContext(SceneContext enterContext)
        {
            EnterContext = enterContext;
        }
    }
}
