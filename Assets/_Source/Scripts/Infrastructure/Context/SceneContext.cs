namespace ITCafe
{
    public abstract class SceneContext
    {
        public string SceneTag { get; }
        public string ToSceneName { get; set; }

        public SceneContext(string sceneTag)
        {
            SceneTag = sceneTag;
        }
    }
}