namespace ITCafe.Infrastructure.Saves
{
    public interface ISaveStateProvider
    {
        public ISaveState SaveState { get; }

        public void SaveAll();
        public void LoadAll();
    }
}