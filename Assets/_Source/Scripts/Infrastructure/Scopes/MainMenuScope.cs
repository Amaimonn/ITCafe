using System;
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
        [SerializeField] private MainMenuView _mainMenuViewPrefab;
        private MainMenuEnterContext _mainMenuEnterContext;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Subject<Unit>>(Lifetime.Scoped).Keyed(Constants.MAIN_MENU_EXIT_SIGNAL);
            
            builder.RegisterInstance<MainMenuView>(_mainMenuViewPrefab);
            builder.Register<MainMenuViewModel>(Lifetime.Scoped);
            builder.Register<Func<MainMenuViewModel>>(x => () => x.Resolve<MainMenuViewModel>(), Lifetime.Singleton);
            builder.Register<LazyAttachBinder<MainMenuView, MainMenuViewModel>>(Lifetime.Singleton)
                .As<IViewBinder<MainMenuView>>();
        }

        public Observable<MainMenuExitContext> Boot(MainMenuEnterContext mainMenuEnterContext = null)
        {
            _mainMenuEnterContext = mainMenuEnterContext;
            Build();

            var rootUIBinder = Container.Resolve<IRootUIBinder>();
            rootUIBinder.ClearViews();

            var mainMenuBinder = Container.Resolve<IViewBinder<MainMenuView>>();
            mainMenuBinder.Open();

            var exitSignal = Container.Resolve<Subject<Unit>>(Constants.MAIN_MENU_EXIT_SIGNAL);
            // define context in UI
            var gameplayEnterContext = new GameplayEnterContext();
            var mainMenuExitContext = new MainMenuExitContext(gameplayEnterContext);
            var mainMenuExitSignal = exitSignal.Select(_ => mainMenuExitContext);

            return mainMenuExitSignal;
        }
    }
}