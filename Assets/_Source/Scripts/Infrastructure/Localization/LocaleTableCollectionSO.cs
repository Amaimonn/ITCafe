// https://github.com/workavast/Blame-Game 2025 Rodion

using DevKit.Utils;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace ITCafe
{
    [CreateAssetMenu(fileName = nameof(LocaleTableCollectionSO),
        menuName = "Scriptable Objects/" + nameof(LocaleTableCollectionSO))]
    public class LocaleTableCollectionSO : ScriptableObject, ILocaleTableCollection
    {
        public InspectorReadonlyList<TableReference> TableReferences => _rawData.TableReferences;
        
        [SerializeField]
        private LocaleTableCollection _rawData;

#if UNITY_EDITOR
        private void OnValidate()
            => _rawData?.UpdateReferences();

        private void OnEnable()
            => _rawData?.UpdateReferences();
#endif
    }
}