namespace ITCafe.Data.Settings
{
    public interface IOptionsSettingData : ISettingBarData
    {
        public string[] Options { get; }
    }
}