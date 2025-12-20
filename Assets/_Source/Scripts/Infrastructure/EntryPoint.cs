using System.Collections;
using DevKit.Utils;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ITCafe
{
    public class EntryPoint
    {
        private static EntryPoint _instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Enter()
        {
            _instance = new();
            _instance.Run();
        }

        private void Run()
        {
            var vContainerSettings = Resources.Load<VContainerSettings>("VContainerSettings"); 
            var instanceProperty = typeof(VContainerSettings).GetProperty("Instance");
            instanceProperty.SetValue(null, vContainerSettings);
            
            var rootScope = vContainerSettings.GetOrCreateRootLifetimeScopeInstance();
            rootScope.Build();
            
            var rootContainer = rootScope.Container;
            var monoHook = rootContainer.Resolve<MonoBehaviourHook>();
            // var loadingScreen = rootContainer.Resolve<LoadingScreen>();
            // loadingScreen.Show();
            
            monoHook.StartCoroutine(LoadEntryScene());
            
            return;

            IEnumerator LoadEntryScene()
            {
                var sceneLoader = rootContainer.Resolve<SceneLoader>();

                yield return sceneLoader.LoadStartScene();
            }
        }
    }
}