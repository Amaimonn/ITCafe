using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Data
{
    [Serializable]
    public class GuideData : IGuideData
    {
        public IReadOnlyList<VisualTreeAsset> Pages => _pages;
        
        [SerializeField] private VisualTreeAsset[] _pages;
    }
}