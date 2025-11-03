using System.Collections;
using DevKit.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using R3;

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
            if (currentScene == Scenes.MAIN_MENU)
                yield return LoadMainMenu(showLoadingImmediately: true);
            else if (currentScene == Scenes.GAMEPLAY)
                yield return LoadGameplay(immediateLoading: true);
#else
            yield return LoadMainMenu(immediateLoading: true)
#endif
        }

        private IEnumerator LoadMainMenu(MainMenuEnterContext mainMenuEnterContext = null, bool showLoadingImmediately = false)
        {
            yield return _loadingScreen.ShowWithInstantlyCoroutine(showLoadingImmediately);
            var startTime = Time.time;
            _onLoadingStarted.OnNext(Unit.Default);

            yield return LoadSceneAsync(Scenes.MAIN_MENU);
            _onLoadingFinished.OnNext(Unit.Default);

            Debug.Log("Main menu scene loaded");

            var mainMenuBootstrap = Object.FindAnyObjectByType<MainMenuScope>();
            var exitMainMenuSignal = mainMenuBootstrap.Boot(mainMenuEnterContext);

            exitMainMenuSignal.Take(1).Subscribe(mainMenuExitContext =>
            {
                _monoHook.StartCoroutine(LoadGameplay(mainMenuExitContext.GameplayEnterContext));
            });

            yield return GetRemainFakeLoadTime(startTime);
            yield return _loadingScreen.HideCoroutine();
        }

        private IEnumerator LoadGameplay(GameplayEnterContext gameplayEnterContext = null, bool immediateLoading = false)
        {
            yield return _loadingScreen.ShowWithInstantlyCoroutine(immediateLoading);

            var startTime = Time.time;
            _onLoadingStarted.OnNext(Unit.Default);

            yield return LoadSceneAsync(Scenes.GAMEPLAY);

            Debug.Log("Gameplay scene loaded");

            var gameplayBootstrap = Object.FindAnyObjectByType<GameplayScope>();
            var gameplayExitSignal = gameplayBootstrap.Boot(gameplayEnterContext);

            gameplayExitSignal.Take(1).Subscribe(gameplayExitContext =>
            {
                _monoHook.StartCoroutine(LoadMainMenu(gameplayExitContext.MainMenuEnterContext));
            });

            _onLoadingFinished.OnNext(Unit.Default);


            yield return GetRemainFakeLoadTime(startTime);
            yield return _loadingScreen.HideCoroutine();
        }


        private IEnumerator LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, mode);
        }

        private YieldInstruction GetRemainFakeLoadTime(float startTime)
        {
            var currentTime = Time.time;
            var remainTime = MIN_LOADING_TIME - (currentTime - startTime);
            if (remainTime > 0)
                return new WaitForSeconds(remainTime);
            else
                return null;
        }
    }
}