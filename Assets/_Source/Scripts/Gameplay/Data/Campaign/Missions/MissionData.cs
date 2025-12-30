using System;
using UnityEngine;

namespace ITCafe.Data.Campaign
{
    [Serializable]
    public class MissionData : IMissionData
    {
        [field: SerializeField] public string Id { get; private set; }

        /// <summary>
        /// The number of the mission that is displayed on the screen.
        /// </summary>
        [field: SerializeField] public string DysplayedNumber { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Text { get; private set; }
        [field: SerializeField] public string SceneName { get; private set; }
    }
}