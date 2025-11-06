
namespace ITCafe
{
    /// <summary>
    /// Info to get back to the MainMenu from the Gameplay scene. Exiting the Gameplay leads to the MainMenu.
    /// </summary>
    public class GameplayExitContext
    {
        public MainMenuEnterContext MainMenuEnterContext { get; }

        public GameplayExitContext(MainMenuEnterContext mainMenuEnterContext)
        {
            MainMenuEnterContext = mainMenuEnterContext;
        }
    }
}
