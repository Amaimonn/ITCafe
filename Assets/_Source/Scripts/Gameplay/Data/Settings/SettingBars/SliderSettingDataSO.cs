using UnityEngine;

namespace ITCafe.Data.Settings
{
    public abstract class SliderSettingDataSO<T> : SettingBarDataSO, ISliderSettingData<T>
    {
        [field: SerializeField] public T MinValue { get; private set; }
        [field: SerializeField] public T MaxValue { get; private set; }
    }
}