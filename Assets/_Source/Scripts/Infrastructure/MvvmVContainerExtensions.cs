using System;
using DevKit.UI;
using DevKit.UI.MVVM;
using DevKit.UI.MVVM.Bases;
using VContainer;
using Object = UnityEngine.Object;

namespace ITCafe
{
    public static class MvvmVContainerExtensions
    {
        public static IContainerBuilder RegisterMVVM<TView, TViewModel>(this IContainerBuilder builder,
            TView viewPrefab, 
            Func<IObjectResolver, TViewModel> viewModelFactory = null,
            Lifetime viewModelLifetime = Lifetime.Singleton,
            Lifetime binderLifetime = Lifetime.Singleton)
            where TView : BaseView, IScreenAttach<TViewModel>
            where TViewModel : IScreenViewModel
        {
            return builder.RegisterMVVM<TView, TViewModel, SimpleAttachBinder<TView, TViewModel>>(
                viewPrefab,
                viewModelFactory,
                viewModelLifetime: viewModelLifetime,
                binderLifetime: binderLifetime);
        }

        public static IContainerBuilder RegisterMVVM<TView, TViewModel, TBinder>(this IContainerBuilder builder,
            TView viewPrefab, 
            Func<IObjectResolver, TViewModel> viewModelFactory = null,
            Lifetime viewModelLifetime = Lifetime.Singleton,
            Lifetime binderLifetime = Lifetime.Singleton)
            where TView : BaseView, IScreenAttach<TViewModel>
            where TViewModel : IScreenViewModel
            where TBinder : IViewBinder<TViewModel>
        {
            builder.Register<Func<TView>>(x => () =>
            {
                var view = Object.Instantiate(viewPrefab);
                x.Inject(view);

                return view;
            }, Lifetime.Singleton);

            builder.Register<TViewModel>(viewModelLifetime);

            var vmFactory = viewModelFactory ?? (x => x.Resolve<TViewModel>());

            builder.Register<Func<TViewModel>>(x => () => vmFactory(x), Lifetime.Singleton);

            builder.Register<TBinder>(binderLifetime)
                .As<IViewBinder<TViewModel>>();

            return builder;
        }
    }
}