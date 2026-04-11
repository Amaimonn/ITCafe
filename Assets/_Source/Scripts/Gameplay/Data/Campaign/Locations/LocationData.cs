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
        public IReadOnlyList<string> MissionIds => AllMissionsDataRaw;
        
        [field: SerializeField] public string[] AllMissionsDataRaw { get; private set; }

    }
}