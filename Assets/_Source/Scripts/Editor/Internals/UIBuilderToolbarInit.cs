#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ITCafe.Editor.Internals
{
    internal class UIBuilderToolbarInit
    {
        [InitializeOnLoadMethod]
        private static void ModifyAddUssPath()
        {
            UIBuilderExtension.ModifyAddUssPath($"{Application.dataPath}/_Source/UIToolkit");
        }
        
        [InitializeOnLoadMethod]
        private static void ModifyCreateNewUssPath()
        {
            UIBuilderExtension.ModifyCreateUssPath($"{Application.dataPath}/_Source/UIToolkit");
        }
    }
}
#endif