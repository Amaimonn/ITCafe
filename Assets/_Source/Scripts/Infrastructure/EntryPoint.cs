using System.Collections;
using DevKit.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
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
            var loadingScreenPrefab = Resources.Load<LoadingScreen>("loading_screen");
            var loadingScreen = Object.Instantiate(loadingScreenPrefab);
            loadingScreen.Show();

            var monoHook = new GameObject("EntryMonoHook").AddComponent<MonoBehaviourHook>();
            Object.DontDestroyOnLoad(monoHook);

            monoHook.StartCoroutine(EntryCoroutine());

            return;

            IEnumerator EntryCoroutine()
            {
                var vContainerSettingsHandle = Addressables.LoadAssetAsync<VContainerSettings>("vcontainer_settings");
                yield return vContainerSettingsHandle;

                var vContainerSettings = vContainerSettingsHandle.Result;
                var instanceProperty = typeof(VContainerSettings).GetProperty("Instance");
                instanceProperty.SetValue(null, vContainerSettings);

                var rootScope = vContainerSettings.GetOrCreateRootLifetimeScopeInstance();
                using (LifetimeScope.Enqueue(builder =>
                   {
                       builder.RegisterInstance(monoHook);
                       builder.RegisterInstance(loadingScreen);
                   }))
                {
                    rootScope.Build();
                }
                
                yield return LocalizationSettings.InitializationOperation;
                
                var rootContainer = rootScope.Container;
                var localizationLoader = rootContainer.Resolve<ILocalizationLoader>();
                
                localizationLoader.Init();
                yield return localizationLoader.LoadTables();
                
                var sceneLoader = rootContainer.Resolve<SceneLoader>();

                yield return sceneLoader.LoadStartScene();
            }
        }
    }
}