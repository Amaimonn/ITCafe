using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Data
{
    [CreateAssetMenu(fileName = nameof(GuideDataSO), menuName = "Scriptable Objects/" + nameof(GuideDataSO))]
    public class GuideDataSO : ScriptableObject, IGuideData
    {
        public IReadOnlyList<VisualTreeAsset> Pages => _rawData.Pages;

        [SerializeField] private GuideData _rawData;
    }
}