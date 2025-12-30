using System;
using System.Collections.Generic;
using System.Linq;
using DevKit.Saves;
using UnityEngine;

namespace ITCafe.Data.Campaign
{
    [Serializable]
    public class CampaignState : IVersioned, ICopyable<CampaignState>
    {
        public int Version { get => _version; set => _version = value; }
        [SerializeField] private int _version = 1;

        public int CampaignDataVersion = 1; // to update the saved state when new missions are added
        public List<LocationState> Locations;
        public string LastLaunchedLocationId;
        public string LastLaunchedMissionId;

        public CampaignState Copy()
        {
            var copiedLocations = Locations?.Select(x => x.Copy()).ToList();
            var copy = (CampaignState)MemberwiseClone();
            copy.Locations = copiedLocations;
            
            return copy;
        }
    }
}