namespace ITCafe.Data.Campaign
{
    public class CafeMissionResult : IMissionResult
    {
        /// <summary>
        /// Appears once after completing all locations.
        /// </summary>
        public bool IsGameCompletion { get; set; } = false;
        public int Stars { get; set; }
    }
}