using DevKit.Newtonsoft;
using ITCafe.Data.Settings;
using Newtonsoft.Json.Linq;

namespace ITCafe.Infrastructure.Saves
{
    // From: {"_version":1,"_settingsState":{"VSync":false,"FPS":-1,"Sensitivity":51,"_version":1}}
    // To: {"Version":2,"SettingsState":{"MusicVolume":80,"SfxVolume":80,"Sensitivity":51,"VSync":false,"FPS":-1,"Brightness":50,"IsPostProcessingEnabled":true,"IsBloomEnabled":true,"IsFilmGrainEnabled":true,"IsChromaticAberrationEnabled":true,"IsAntiAliasingEnabled":true,"ScreenWidth":2560,"ScreenHeight":1440,"Fullscreen":true,"Language":"en","Version":2},"CampaignState":{"CampaignDataVersion":1,"Locations":[{"Id":"location_1","IsCompleted":false,"OpenedMissions":[{"Id":"mission_1_1","IsCompleted":false,"Stars":0}],"MaxCompletedMissionId":null}],"LastLaunchedLocationId":"","LastLaunchedMissionId":"","Version":1}}
    public class MigrationV1ToV2 : IMigration
    {
        public int ToVersion => 2;

        public JObject Migrate(JObject dataObject)
        {
            var settingsStateToken = dataObject["_settingsState"];
            var settingsState = new SettingsState();

            if (settingsStateToken != null)
            {
                var vSync = settingsStateToken["VSync"];
                if (vSync != null)
                    settingsState.VSync = vSync.ToObject<bool>();

                var fps = settingsStateToken["FPS"];
                if (fps != null)
                    settingsState.FPS = fps.ToObject<int>();

                var sensitivity = settingsStateToken["Sensitivity"];
                if (sensitivity != null)
                    settingsState.Sensitivity = sensitivity.ToObject<int>();
                
                dataObject.Remove("_settingsState");
            }
            
            dataObject["SettingsState"] = JToken.FromObject(settingsState);
            
            var campaignState = new JObject
            {
                ["Version"] = 1,
                ["CampaignDataVersion"] = 1,
                ["Locations"] = new JArray
                {
                    new JObject
                    {
                        ["Id"] = "location_1",
                        ["IsCompleted"] = false,
                        ["OpenedMissions"] = new JArray
                        {
                            new JObject
                            {
                                ["Id"] = "mission_1_1",
                                ["IsCompleted"] = false,
                                ["Stars"] = 0
                            }
                        }
                    }
                },
                ["LastLaunchedLocationId"] = "",
                ["LastLaunchedMissionId"] = ""
            };
            
            dataObject["CampaignState"] = campaignState;
            
            return dataObject;
        }
    }
}