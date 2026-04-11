using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ITCafe.Data
{
    public interface IGuideData
    {
        public IReadOnlyList<VisualTreeAsset> Pages { get; }
    }
}