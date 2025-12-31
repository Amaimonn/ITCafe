using System.Collections.Generic;
using UnityEngine;

namespace ITCafe.Data.Campaign
{
    [CreateAssetMenu(fileName = "LocationDataSO", menuName = "Scriptable Objects/LocationDataSO")]
    public class LocationDataSO : ScriptableObject, ILocationData
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        public IReadOnlyList<string> MissionIds => _missionIds;

        [SerializeField] private string[] _missionIds;
    }
}