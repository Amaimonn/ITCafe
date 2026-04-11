using System.Collections.Generic;
using DevKit.Saves;

namespace ITCafe.Data.Campaign
{
    public interface ILocationDataCollection : IVersioned
    {
        public IReadOnlyList<ILocationData> AllData { get; }
    }
}