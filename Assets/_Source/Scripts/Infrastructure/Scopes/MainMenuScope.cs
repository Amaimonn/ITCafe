using System;
using System.Collections;
using System.Collections.Generic;
using DevKit.UI.MVVM;
using DevKit.UI.MVVM.Bases;
using ITCafe.Gameplay.UI.MVVM;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ITCafe
{
    public class MainMenuScope : LifetimeScope
    {
        public Observable<MainMenuExitContext> ExitSignal { get; private set; }
        
        [SerializeField] private MainMenuView _mainMenuViewPrefab;
        
        private MainMenuEnterContext _mainMenuEnterContext;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Subject<Unit>>(Lifetime.Scoped)
                .Keyed(Constants.MAIN_MENU_EXIT_SIGNAL);

            builder.RegisterInstance<MainMenuView>(_mainMenuViewPrefab);
            builder.Register<MainMenuViewModel>(Lifetime.Scoped);
            builder.Register<Func<MainMenuViewModel>>(x => () => x.Resolve<MainMenuViewModel>(), Lifetime.Singleton);
            builder.Register<SimpleAttachBinder<MainMenuView, MainMenuViewModel>>(Lifetime.Singleton)
                .As<IViewBinder<MainMenuViewModel>>();
        }

        public IEnumerator BootCoroutine(MainMenuEnterContext mainMenuEnterContext = null)
        {
            Time.timeScale = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            _mainMenuEnterContext = mainMenuEnterContext;
            
            Build();
            yield return new WaitForEndOfFrame();

            var rootUIBinder = Container.Resolve<IRootUIBinder>();
            rootUIBinder.ClearViews();

            var mainMenuBinder = Container.Resolve<IViewBinder<MainMenuViewModel>>();
            mainMenuBinder.Open();

            var exitSignal = Container.Resolve<Subject<Unit>>(Constants.MAIN_MENU_EXIT_SIGNAL);
            var gameplayEnterContext = new GameplayEnterContext()
            {
                // TODO: define through UI
                ToSceneName = Scenes.GAMEPLAY_1, 
                LocationId = "location_1",
                MissionId = "mission_1_1"
            };
            var mainMenuExitContext = new MainMenuExitContext(gameplayEnterContext);
            var mainMenuExitSignal = exitSignal.Select(_ => mainMenuExitContext);

            ExitSignal = mainMenuExitSignal;
        }
    }
}
