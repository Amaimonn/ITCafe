using Cysharp.Threading.Tasks;
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
            var vContainerSettings = Resources.Load<VContainerSettings>("vcontainer_settings"); 
            var instanceProperty = typeof(VContainerSettings).GetProperty("Instance");
            instanceProperty.SetValue(null, vContainerSettings);
            
            var rootScope = vContainerSettings.GetOrCreateRootLifetimeScopeInstance();
            rootScope.Build();
            
            var rootContainer = rootScope.Container;
            var loadingScreen = rootContainer.Resolve<LoadingScreen>();
            loadingScreen.Show();
            
            var sceneLoader = rootContainer.Resolve<SceneLoader>();
            sceneLoader.LoadStartScene().ToUniTask();
        }
    }
}