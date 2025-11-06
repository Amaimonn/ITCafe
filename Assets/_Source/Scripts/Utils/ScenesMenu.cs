#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ITCafe.Editor
{
    public static class ScenesMenu
    {
        [MenuItem("Scenes/MainMenu")]
        private static void MainMenu()
        {
            OpenScene(Scenes.MAIN_MENU);
        }

        [MenuItem("Scenes/Gameplay")]
        private static void Gameplay()
        {
            OpenScene(Scenes.GAMEPLAY);
        }

        private static void OpenScene(string sceneRelativePath)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene($"Assets/_Source/Scenes/{sceneRelativePath}.unity");
        }
    }
}
#endif