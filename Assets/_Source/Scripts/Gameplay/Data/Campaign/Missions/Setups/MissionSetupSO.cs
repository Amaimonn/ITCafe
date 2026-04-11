using ITCafe.Data.Items;
using UnityEngine;

namespace ITCafe.Data
{
    [CreateAssetMenu(fileName = nameof(MissionSetupSO), menuName = "Scriptable Objects/" + nameof(MissionSetupSO))]
    public class MissionSetupSO : ScriptableObject, IMissionSetup
    {
        [field: SerializeField] public GameObject SceneSetupPrefab { get; private set; }

        public IItemInfoCollection ItemInfoCollection => _itemInfoCollection;
        [SerializeField] private ItemInfoCollection _itemInfoCollection;

        public IRecipeCollection RecipeCollection => _recipesCollection;
        [SerializeField] private RecipeCollection _recipesCollection;

        public IGuideData GuideData => _guideData;
        [SerializeField] private GuideData _guideData;

        public IMissionEvaluation MissionEvaluation => _missionEvaluation;
        [SerializeField] private MissionEvaluation _missionEvaluation;

        public ILocaleTableCollection LocaleTableCollection => _localeTableCollection;
        [SerializeField] private LocaleTableCollection _localeTableCollection;

#if UNITY_EDITOR
        private void OnEnable()
        {
            _localeTableCollection?.UpdateReferences();
        }

        private void OnValidate()
        {
            _localeTableCollection?.UpdateReferences();
        }
#endif
    }
}