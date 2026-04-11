#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;

namespace ITCafe.Editor
{
    public static class ScenesMenu
    {
        private const string SCENES_PATH = "Assets/_Source/Scenes";

        [MainToolbarElement("Scenes")]
        public static MainToolbarElement ScenesButton()
        {
            var content = new MainToolbarContent("Scenes");
            var dropdown = new MainToolbarDropdown(content, (r) => ShowMenu());

            return dropdown;
        }

        private static void ShowMenu()
        {
            var menu = new GenericMenu();
            var sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { SCENES_PATH });

            if (sceneGuids.Length == 0)
            {
                menu.AddDisabledItem(new GUIContent("No scenes found"));
            }
            else
            {
                var scenes = sceneGuids
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .OrderBy(Path.GetFileNameWithoutExtension)
                    .ToArray();

                foreach (var scenePath in scenes)
                {
                    var sceneName = Path.GetFileNameWithoutExtension(scenePath);
                    menu.AddItem(new GUIContent(sceneName), false, () => OpenScene(scenePath));
                }
            }

            menu.ShowAsContext();
        }

        private static void OpenScene(string scenePath)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}
#endif