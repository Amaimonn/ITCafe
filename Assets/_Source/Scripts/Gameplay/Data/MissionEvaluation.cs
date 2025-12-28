using System;
using System.Linq;
using DevKit.Utils;
using UnityEngine;

namespace ITCafe.Gameplay.Data
{
    [Serializable]
    public class MissionEvaluation
    {
        [field: SerializeField] public InspectorReadonlyList<int> StarEvaluations { get; private set; } =
            Enumerable.Range(0, Constants.STAR_COUNT).Select(_ => 0).ToInspectorReadonlyList();
    }
}