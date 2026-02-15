using System;
using DevKit.Utils;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace ITCafe
{
    [Serializable]
    public class LocaleTableCollection : ILocaleTableCollection
    {
        [field: SerializeField, InspectorReadOnly]
        public InspectorReadonlyList<TableReference> TableReferences { get; private set; }

#if UNITY_EDITOR
        [SerializeField] private UnityEditor.Localization.StringTableCollection[] _tables;
#endif
        
        public void UpdateReferences()
        {
#if UNITY_EDITOR
            if (_tables == null)
                return;

            TableReferences.Clear();
            TableReferences.Capacity = _tables.Length;
            
            foreach (var table in _tables)
            {
                if (table != null)
                    TableReferences.Add(table.TableCollectionName);
            }
#endif
        }
    }
}