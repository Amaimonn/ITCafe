using System.Collections.Generic;

namespace ITCafe.Data.Campaign
{
    public interface ILocationData
    {
        public string Id { get; }
        public string Name { get; }
        
        public IReadOnlyList<IMissionData> AllMissionsData { get; }
    }
}