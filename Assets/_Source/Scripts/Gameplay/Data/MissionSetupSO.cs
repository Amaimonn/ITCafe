using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Gameplay.Data
{
    [CreateAssetMenu(fileName = "MissionSetupSO", menuName = "Scriptable Objects/MissionSetupSO")]
    public class MissionSetupSO : ScriptableObject
    {
        [field: SerializeField] public AllItemInfoSO ItemsInfoSO { get; private set; }
        [field: SerializeField] public AllRecipesSO RecipesSO { get; private set; }
        [field: SerializeField] public GuideSO GuideSO { get; private set; }
        [field: SerializeField] public MissionEvaluation MissionEvaluation { get; private set; }
    }
}