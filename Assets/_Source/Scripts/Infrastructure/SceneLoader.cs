using System.Collections;
using System.Collections.Generic;
using DevKit.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using R3;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace ITCafe
{
    public class SceneLoader
    {
        public Observable<Unit> OnLoadingStarted => _onLoadingStarted;
        public Observable<Unit> OnLoadingFinished => _onLoadingFinished;

        private readonly MonoBehaviourHook _monoHook;
        private readonly LoadingScreen _loadingScreen;
        private readonly Subject<Unit> _onLoadingStarted = new();
        private readonly Subject<Unit> _onLoadingFinished = new();
        private const float MIN_LOADING_TIME = 1f;

        public SceneLoader(MonoBehaviourHook hook, LoadingScreen loadingScreen)
        {
            _monoHook = hook;
            _loadingScreen = loadingScreen;
        }

        public IEnumerator LoadStartScene()
        {
            var currentScene = SceneManager.GetActiveScene().name;
#if UNITY_EDITOR
            switch (currentScene)
            {
                case Scenes.MAIN_MENU:
                    yield return LoadMainMenu(showLoadingImmediately: true);
                    break;
                case Scenes.GAMEPLAY_1:
                    yield return LoadGameplay(immediateLoading: true);
                    break;
            }
#else
            yield return LoadMainMenu(showLoadingImmediately: true);
#endif
        }

        private IEnumerator LoadMainMenu(MainMenuEnterContext mainMenuEnterContext = null,
            bool showLoadingImmediately = false)
        {
            yield return _loadingScreen.ShowWithInstantCoroutine(showLoadingImmediately);

            var startTime = Time.time;
            _onLoadingStarted.OnNext(Unit.Default);

            yield return LoadSceneAsync(Scenes.GAP);
            yield return LoadSceneAsync(Scenes.MAIN_MENU);

            _onLoadingFinished.OnNext(Unit.Default);

            Debug.Log("Main menu scene loaded");

            var mainMenuBootstrap = Object.FindAnyObjectByType<MainMenuScope>();
            yield return mainMenuBootstrap.BootCoroutine(mainMenuEnterContext);

            var exitMainMenuSignal = mainMenuBootstrap.ExitSignal;

            exitMainMenuSignal.Take(1).Subscribe(mainMenuExitContext =>
            {
                _monoHook.StartCoroutine(LoadGameplay(mainMenuExitContext.GameplayEnterContext));
            });

            yield return GetRemainFakeLoadTime(startTime);
            yield return _loadingScreen.HideCoroutine();
        }

        private IEnumerator LoadGameplay(GameplayEnterContext gameplayEnterContext = null, bool immediateLoading = false)
        {
            yield return _loadingScreen.ShowWithInstantCoroutine(immediateLoading);

            var startTime = Time.time;
            _onLoadingStarted.OnNext(Unit.Default);

            yield return LoadSceneAsync(Scenes.GAP);
            yield return LoadSceneAsync(gameplayEnterContext == null ? Scenes.GAMEPLAY_1 : 
                gameplayEnterContext.ToSceneName);

            Debug.Log("Gameplay scene loaded");

            var gameplayBootstrap = Object.FindAnyObjectByType<GameplayScope>();
            yield return gameplayBootstrap.BootCoroutine(gameplayEnterContext);

            var gameplayExitSignal = gameplayBootstrap.ExitSignal;

            gameplayExitSignal.Take(1).Subscribe(gameplayExitContext =>
            {
                var enterContext = gameplayExitContext.EnterContext;
                switch (enterContext.SceneTag)
                {
                    case Scenes.MAIN_MENU:
                        _monoHook.StartCoroutine(LoadMainMenu((MainMenuEnterContext)enterContext));
                        break;
                    case Scenes.GAMEPLAY:
                        _monoHook.StartCoroutine(LoadGameplay((GameplayEnterContext)enterContext));
                        break;
                    default:
                        FLogger.LogError<SceneLoader>("Unhandled scene enter context");
                        _monoHook.StartCoroutine(LoadMainMenu());
                        break;
                }
            });

            _onLoadingFinished.OnNext(Unit.Default);

            yield return GetRemainFakeLoadTime(startTime);
            yield return _loadingScreen.HideCoroutine();
        }

        private AsyncOperationHandle<SceneInstance> LoadSceneAsync(string sceneName,
            LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            var handle = Addressables.LoadSceneAsync($"scenes/{sceneName}", loadMode, activateOnLoad: true);
            handle.Destroyed += _ => FLogger.LogGood<SceneLoader>($"Scene {sceneName} was unloaded");

            return handle;
        }

        private IEnumerator GetRemainFakeLoadTime(float startTime)
        {
            var currentTime = Time.time;
            var remainTime = MIN_LOADING_TIME - (currentTime - startTime);
            if (remainTime > 0)
                yield return new WaitForSeconds(remainTime);
            else
                yield return null;
        }
    }
}