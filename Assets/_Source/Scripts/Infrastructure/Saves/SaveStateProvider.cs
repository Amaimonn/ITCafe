using System.Collections.Generic;
using DevKit.Saves;
using DevKit.Utils;
using ITCafe.Data;
using ITCafe.Data.Campaign;

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
                _saveState = new SaveStateV1
                {
                    SettingsState = new SettingsState
                    {
                        Sensitivity = 50,
                        VSync = false,
                        FPS = -1,
                    },
                    CampaignState = new CampaignState
                    {
                        Locations = new List<LocationState>
                        {
                            new("location_1", false, new List<MissionState>
                            {
                                new("mission_1_1", false)
                            })
                        }
                    }
                };

                SaveAll();
                FLogger.Log<SaveStateProvider>("New save file was created");
            }
        }

        public void SaveAll()
        {
            _saveSystem.Save(SAVE_STATE, _saveState);
            FLogger.Log<SaveStateProvider>("State was saved");
        }
    }
}