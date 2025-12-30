using UnityEngine;

namespace ITCafe.Data.Campaign
{
    [CreateAssetMenu(fileName = "MissionDataSO", menuName = "Scriptable Objects/MissionDataSO")]
    public class MissionDataSO : ScriptableObject, IMissionData
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string DysplayedNumber { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea(6, 20)] public string Text { get; private set; }
        [field: SerializeField] public string SceneName { get; private set; }
    }
}
