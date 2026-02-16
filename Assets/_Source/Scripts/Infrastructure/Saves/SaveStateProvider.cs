using System.Collections.Generic;
using DevKit.Newtonsoft;
using DevKit.Saves;
using DevKit.Utils;
using ITCafe.Data.Settings;
using ITCafe.Data.Campaign;

namespace ITCafe.Infrastructure.Saves
{
    public class SaveStateProvider : ISaveStateProvider
    {
        public ISaveState SaveState => _saveState;
        private SaveStateV2 _saveState;
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
                var saveFileFata = _saveSystem.LoadRaw(SAVE_STATE);
                var migrator = new Migrator(new MigrationV1ToV2());

                if (migrator.TryMigrateIfNecessary<SaveStateV2>(saveFileFata, out _saveState))
                    SaveAll();
            }
            else
            {
                _saveState = new SaveStateV2
                {
                    SettingsState = new SettingsState(),
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