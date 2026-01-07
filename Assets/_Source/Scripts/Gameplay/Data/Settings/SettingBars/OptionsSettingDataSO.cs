using UnityEngine;

namespace ITCafe.Data.Settings
{
    [CreateAssetMenu(fileName = "OptionsSettingDataSO", 
        menuName = "Scriptable Objects/Settings/OptionsSettingDataSO")]
    public class OptionsSettingDataSO : SettingBarDataSO, IOptionsSettingData
    {
        [field: SerializeField] public virtual string[] Options { get; private set; }
    }
}