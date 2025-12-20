using DevKit.Saves;

namespace ITCafe.Infrastructure.Saves
{
    public class SaveStateProvider : ISaveStateProvider
    {
        public ISaveState SaveState => _saveState;
        private SaveStateV1 _saveState;
        private readonly ISaveSystem _saveSystem;
        private const string SAVE_STATE = "SAVE_STATE";

        public SaveStateProvider(ISaveSystem saveSystem)
        {
            _saveSystem = saveSystem;
        }

        public void LoadAll()
        {
            if (_saveSystem.Exists(SAVE_STATE))
            {
                _saveState = _saveSystem.Load<SaveStateV1>(SAVE_STATE);
            }
            else 
            {
                _saveState = new()
                {
                    SettingsState = new()
                    {
                        Sensitivity = 50,
                        VSync = false,
                        FPS = -1,
                    }
                };

                SaveAll();
            }
        }

        public void SaveAll()
        {
            _saveSystem.Save(SAVE_STATE, _saveState);
        }
    }
}