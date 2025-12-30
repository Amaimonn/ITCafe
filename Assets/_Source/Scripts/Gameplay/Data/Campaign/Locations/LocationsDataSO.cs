using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace ITCafe.Data.Campaign
{
    [CreateAssetMenu(fileName = "AllLocationsDataSO", menuName = "Scriptable Objects/AllLocationsDataSO")]
    public class AllLocationsDataSO : ScriptableObject, IAllLocationsData
    {
        [field: SerializeField] public int Version { get; private set; }
        public IReadOnlyList<ILocationData> AllData => _locationsData;

        [SerializeField] private LocationDataSO[] _locationsData;
    }
}
