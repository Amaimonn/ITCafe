namespace ITCafe.Data.Campaign
{
    public interface IMissionData
    {
        public string Id { get; }

        /// <summary>
        /// The number of the mission in UI.
        /// </summary>
        public string DisplayedNumber { get; }
        public string Name { get; }
        public string Description { get; }
        public string SceneName { get; }
    }
}