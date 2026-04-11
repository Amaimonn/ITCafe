#if UNITY_EDITOR
using Unity.UI.Builder;
using UnityEditor;

namespace ITCafe.Editor.Internals
{
    internal class UIBuilderExtension
    {
        public static void ModifyAddUssPath(string path)
        {
            EditorApplication.delayCall += () =>
            {
                BuilderStyleSheetsUtilities.s_OpenFileDialogCallback = () =>
                    BuilderDialogsUtility.DisplayOpenFileDialog("Open USS File", path, "uss");
            };
        }
        
        public static void ModifyCreateUssPath(string path)
        {
            EditorApplication.delayCall += () =>
            {
                BuilderStyleSheetsUtilities.s_SaveFileDialogCallback = () =>
                    BuilderDialogsUtility.DisplaySaveFileDialog("Save USS File", path, (string) null, "uss");
            };
        }
    }
}
#endif