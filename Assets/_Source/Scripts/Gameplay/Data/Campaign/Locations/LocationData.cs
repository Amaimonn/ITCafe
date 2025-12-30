using System;
using System.Collections.Generic;
using UnityEngine;

namespace ITCafe.Data.Campaign
{
    [Serializable]
    public class LocationData : ILocationData
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        public IReadOnlyList<IMissionData> AllMissionsData => AllMissionsDataRaw;
        
        [field: SerializeField] public MissionData[] AllMissionsDataRaw { get; private set; }

    }
}