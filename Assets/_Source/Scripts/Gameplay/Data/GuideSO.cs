using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Data
{
    [CreateAssetMenu(fileName = "GuideSO", menuName = "Scriptable Objects/GuideSO")]
    public class GuideSO : ScriptableObject
    {
        public IReadOnlyList<VisualTreeAsset> Pages => _pages;
        
        [SerializeField] private VisualTreeAsset[] _pages;
    }
}