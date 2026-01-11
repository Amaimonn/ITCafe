// https://github.com/workavast/Blame-Game 2025 Rodion

using DevKit.Utils;
using UnityEngine;
using UnityEngine.Localization.Tables;

#if UNITY_EDITOR
using UnityEditor.Localization;
#endif

namespace ITCafe
{
    [CreateAssetMenu(fileName = nameof(StringTablesConfig),
        menuName = "Scriptable Objects/" + nameof(StringTablesConfig))]
    public class StringTablesConfig : ScriptableObject
    {
        [field: SerializeField, InspectorReadOnly]
        public InspectorReadonlyList<TableReference> TableReferences { get; private set; }

#if UNITY_EDITOR
        [SerializeField] private StringTableCollection[] tables;

        private void OnValidate()
            => UpdateReferences();

        private void OnEnable()
            => UpdateReferences();

        private void UpdateReferences()
        {
            if (tables == null)
                return;

            TableReferences.Clear();
            TableReferences.Capacity = tables.Length;
            foreach (var table in tables)
            {
                if (table != null)
                    TableReferences.Add(table.TableCollectionName);
            }
        }
#endif
    }
}