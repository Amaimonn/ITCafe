using System.Collections.Generic;
using UnityEngine;

namespace ITCafe.Data.Campaign
{
    [CreateAssetMenu(fileName = nameof(LocationDataCollectionSO),
        menuName = "Scriptable Objects/" + nameof(LocationDataCollectionSO))]
    public class LocationDataCollectionSO : ScriptableObject, ILocationDataCollection
    {
        [field: SerializeField] public int Version { get; private set; }
        public IReadOnlyList<ILocationData> AllData => _locationsData;

        [SerializeField] private LocationDataSO[] _locationsData;
    }
}