using ITCafe.Gameplay.Data;

namespace ITCafe.Infrastructure.Saves
{
    public interface ISaveState
    {
        public SettingsState SettingsState { get; }
    }
}