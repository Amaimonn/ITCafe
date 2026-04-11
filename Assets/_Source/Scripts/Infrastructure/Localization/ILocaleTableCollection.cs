using DevKit.Utils;
using UnityEngine.Localization.Tables;

namespace ITCafe
{
    public interface ILocaleTableCollection
    {
        InspectorReadonlyList<TableReference> TableReferences { get; }
    }
}