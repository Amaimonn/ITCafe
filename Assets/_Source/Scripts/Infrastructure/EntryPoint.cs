using System.Collections;
using DevKit.UI.MVVM;
using DevKit.Utils;
using UnityEngine;
using VContainer;
using VContainer.Unity;

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
            
            var vContainerSettings = Resources.Load<VContainerSettings>("VContainerSettings"); 

            var rootScope = vContainerSettings.GetOrCreateRootLifetimeScopeInstance();
#if UNITY_EDITOR
            VContainerSettings.LoadInstanceFromPreloadAssets();
            rootScope = VContainerSettings.Instance.GetOrCreateRootLifetimeScopeInstance();
#endif
            rootScope.Build();
            
            var rootContainer = rootScope.Container;
            var monoHook = rootContainer.Resolve<MonoBehaviourHook>();
            var loadingScreen = rootContainer.Resolve<LoadingScreen>();
            loadingScreen.Show();
            
            monoHook.StartCoroutine(LoadEntryScene());

            IEnumerator LoadEntryScene()
            {
                // var rootUIPrefab = Resources.Load<RootUIBinder>("RootUIBinder");
                // var rootUIBinder = Object.Instantiate(rootUIPrefab);
                // Object.DontDestroyOnLoad(rootUIBinder);
                //
                // var loadingScreen = rootUIBinder.GetComponentInChildren<LoadingScreen>();

                var sceneLoader = rootContainer.Resolve<SceneLoader>();

                yield return sceneLoader.LoadStartScene();
            }
        }
    }
}