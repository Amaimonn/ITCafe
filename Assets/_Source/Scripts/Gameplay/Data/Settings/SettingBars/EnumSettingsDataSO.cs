using UnityEngine;

namespace ITCafe.Data.Settings
{
    [CreateAssetMenu(fileName = "EnumSettingsDataSO", 
        menuName = "Scriptable Objects/Settings/EnumSettingsDataSO")]
    public class EnumSettingsDataSO : OptionsSettingDataSO
    {
        [field: SerializeField] public override string[] Options => _options;
        [field: SerializeField] public virtual string[] OverrideDisplayOptions => _overrideOptions;
        
        [SerializeField] private string[] _options;
        [SerializeField] private string[] _overrideOptions;
    }
}