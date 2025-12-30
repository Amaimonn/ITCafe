using System.Collections.Generic;
using DevKit.Saves;

namespace ITCafe.Data.Campaign
{
    public interface IAllLocationsData : IVersioned
    {
        public IReadOnlyList<ILocationData> AllData { get; }
    }
}