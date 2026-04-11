using System.Collections.Generic;
using UnityEngine;

namespace ITCafe.Data.Campaign
{
    [CreateAssetMenu(fileName = "MissionDataSO", menuName = "Scriptable Objects/MissionDataSO")]
    public class MissionDataSO : ScriptableObject, IMissionData
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string DisplayedNumber { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea(6, 20)] public string Description { get; private set; }
        [field: SerializeField] public string SceneName { get; private set; }
        [field: SerializeField] public int PositionX { get; private set; }
        [field: SerializeField] public int PositionY { get; private set; }
        public IReadOnlyList<string> NextMissionIds => _nextMissionIds;
        public IReadOnlyList<string> NextLocationIds => _nextLocationIds;
        [field: SerializeField] private string[] _nextMissionIds;
        [field: SerializeField] private string[] _nextLocationIds;
    }
}
