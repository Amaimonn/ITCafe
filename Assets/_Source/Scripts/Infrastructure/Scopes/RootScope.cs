using DevKit.UI.MVVM;
using DevKit.Utils;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ITCafe
{
    public class RootScope : LifetimeScope
    {
        [SerializeField] private RootUIBinder _rootUIBinderPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            FLogger.Log("RootScope Configure");
            var rootUIBinder = Instantiate(_rootUIBinderPrefab);
            DontDestroyOnLoad(rootUIBinder);
            builder.RegisterInstance<RootUIBinder>(rootUIBinder)
                .AsSelf()
                .As<IRootUIBinder>();

            var monoHook = new GameObject("EntryMonoHook").AddComponent<MonoBehaviourHook>();
            DontDestroyOnLoad(monoHook);
            builder.RegisterInstance<MonoBehaviourHook>(monoHook);

            var loadingScreen = rootUIBinder.gameObject.GetComponentInChildren<LoadingScreen>(includeInactive: true);
            if (loadingScreen == null)
            {
                FLogger.LogError("Loading Screen not found");
            }
            builder.RegisterComponent<LoadingScreen>(loadingScreen);

            builder.Register<SceneLoader>(Lifetime.Singleton);
        }
    }
}