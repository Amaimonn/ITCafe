namespace ITCafe.Data.Campaign
{
    public interface IMissionData
    {
        public string Id { get; }

        /// <summary>
        /// The number of the mission in UI.
        /// </summary>
        public string DysplayedNumber { get; }
        public string Name { get; }
        public string Text { get; }
        public string SceneName { get; }
    }
}