namespace ITCafe
{
    /// <summary>
    /// Info to enter the Gameplay scene from the Main menu. Exiting the Main menu leads to the Gameplay.
    /// </summary>
    public class MainMenuExitContext
    {
        public GameplayEnterContext GameplayEnterContext { get; }

        public MainMenuExitContext(GameplayEnterContext gameplayEnterContext)
        {
            GameplayEnterContext = gameplayEnterContext;
        }
    }
}
