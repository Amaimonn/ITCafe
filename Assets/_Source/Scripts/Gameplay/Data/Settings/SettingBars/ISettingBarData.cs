namespace ITCafe.Data.Settings
{
    public interface ISettingBarData
    {
        public string Label { get; }
        public string Description { get; }
        public string WarningText { get; }
    }
}