namespace ITCafe
{
    public abstract class SceneContext
    {
        public string SceneName { get; }

        public SceneContext(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}
