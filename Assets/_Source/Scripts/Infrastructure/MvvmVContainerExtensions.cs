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
            TView viewPrefab, Lifetime viewModelLifetime)
            where TView : BaseView, IScreenAttach<TViewModel>
            where TViewModel : IScreenViewModel
        {
            return builder.RegisterMVVM<TView, TViewModel, SimpleAttachBinder<TView, TViewModel>>(
                viewPrefab, 
                null,
                viewModelLifetime);
        }

        public static IContainerBuilder RegisterMVVM<TView, TViewModel, TBinder>(this IContainerBuilder builder,
            TView viewPrefab, Lifetime viewModelLifetime)
            where TView : BaseView, IScreenAttach<TViewModel>
            where TViewModel : IScreenViewModel
            where TBinder : IViewBinder<TViewModel>
        {
            return builder.RegisterMVVM<TView, TViewModel, TBinder>(
                viewPrefab, 
                null,
                viewModelLifetime);
        }

        public static IContainerBuilder RegisterMVVM<TView, TViewModel>(this IContainerBuilder builder,
            TView viewPrefab, Func<IObjectResolver, TViewModel> viewModelFactory = null,
            Lifetime viewModelLifetime = Lifetime.Singleton)
            where TView : BaseView, IScreenAttach<TViewModel>
            where TViewModel : IScreenViewModel
        {
            return builder.RegisterMVVM<TView, TViewModel, SimpleAttachBinder<TView, TViewModel>>(
                viewPrefab,
                viewModelFactory,
                viewModelLifetime);
        }

        public static IContainerBuilder RegisterMVVM<TView, TViewModel, TBinder>(this IContainerBuilder builder,
            TView viewPrefab, Func<IObjectResolver, TViewModel> viewModelFactory = null,
            Lifetime viewModelLifetime = Lifetime.Singleton)
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

            builder.Register<TBinder>(Lifetime.Singleton)
                .As<IViewBinder<TViewModel>>();

            return builder;
        }
    }
}