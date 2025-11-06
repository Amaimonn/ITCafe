using DevKit.UI.MVVM;
using ITCafe.Gameplay.UI.MVVM;
using R3;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace ITCafe
{
    public class MainMenuScope : LifetimeScope
    {
        [SerializeField] private MainMenuView _mainMenuView;
        private MainMenuEnterContext _mainMenuEnterContext;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Subject<Unit>>(Lifetime.Scoped).Keyed(Constants.MAIN_MENU_EXIT_SIGNAL);
            builder.RegisterInstance<MainMenuView>(_mainMenuView);
            builder.Register<MainMenuViewModel>(Lifetime.Scoped);
        }
        
        public Observable<MainMenuExitContext> Boot(MainMenuEnterContext mainMenuEnterContext = null)
        {
            _mainMenuEnterContext = mainMenuEnterContext;
            Build();
            
            var mainMenuViewModel = Container.Resolve<MainMenuViewModel>();
            var mainMenuElement = _mainMenuView.InitAndGetRoot();
            var rootUIBinder = Container.Resolve<RootUIBinder>();
            _mainMenuView.Bind(mainMenuViewModel);
            rootUIBinder.SetView(_mainMenuView);
            
            var exitSignal = Container.Resolve<Subject<Unit>>(Constants.MAIN_MENU_EXIT_SIGNAL); // for MainMenuViewModel
            // define context in UI
            var gameplayEnterContext = new GameplayEnterContext();
            var mainMenuExitContext = new MainMenuExitContext(gameplayEnterContext);
            var mainMenuExitSignal = exitSignal.Select(_ => mainMenuExitContext);

            return mainMenuExitSignal;
        }
    }
}