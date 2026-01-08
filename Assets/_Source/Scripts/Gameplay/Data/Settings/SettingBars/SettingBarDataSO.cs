using UnityEngine;

namespace ITCafe.Data.Settings
{
    public abstract class SettingBarDataSO : ScriptableObject, ISettingBarData
    {
        [field: SerializeField] public string Label { get; private set; }
        [field: SerializeField] public string WarningText { get; private set; }
    }
}