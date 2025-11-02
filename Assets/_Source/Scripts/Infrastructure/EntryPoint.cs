using System.Collections;
using DevKit.Utils;
using UnityEngine;

namespace ITCafe
{
    public class GameEntryPoint
    {
        private static GameEntryPoint _instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Enter()
        {
            _instance = new();
            _instance.Run();
        }

        private void Run()
        {
            var monoHook = new GameObject("EntryMonoHook").AddComponent<MonoBehaviourHook>();
            Object.DontDestroyOnLoad(monoHook);
            monoHook.StartCoroutine(LoadEntryScene());
            
            IEnumerator LoadEntryScene()
            {
                var rootUIPrefab = Resources.Load<GameObject>("RootUIBinder");
                var rootUIBinder = Object.Instantiate(rootUIPrefab);
                Object.DontDestroyOnLoad(rootUIBinder);
                
                var loadingScreen = rootUIBinder.GetComponentInChildren<LoadingScreen>();
                var sceneLoader = new SceneLoader(monoHook, loadingScreen);
                
                yield return sceneLoader.LoadStartScene();
            }
        }
    }
}