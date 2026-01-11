using System;
using System.Collections.Generic;
using System.Linq;
using DevKit.UI.MVVM;
using R3;
using DevKit.Utils;
using ITCafe.Data.Settings;

namespace ITCafe.Gameplay.UI.MVVM
{
    public abstract class SettingsSectionViewModel : IViewModel, IDisposable
    {
        public ReadOnlyReactiveProperty<bool> IsAnyChanges = new ReactiveProperty<bool>(false);

        protected SettingsModel _model;
        protected ISettingsData _data;
        protected CompositeDisposable _disposables;
        private readonly List<IReactiveChange> _controlReactiveChanges = new();
        
        public void Bind(SettingsModel model)
        {
            _model = model;
            _disposables = new();

            OnBind(model);
            TrackChanges();
        }

        public void SetSettingsData(ISettingsData data)
        {
            _data = data;
        }

        protected virtual void OnBind(SettingsModel model)
        {
        }

        private void TrackChanges()
        {
            IsAnyChanges = Observable.CombineLatest(_controlReactiveChanges.Select(x => x.IsChanged))
                .Select(x => x.Any(t => t))
                .ToReadOnlyReactiveProperty();
        }

        public virtual void ApplyChanges()
        {
            foreach (var reactiveChange in _controlReactiveChanges)
                reactiveChange.ApplyChanges();
        }

        public virtual void CancelChanges()
        {
            foreach (var reactiveChange in _controlReactiveChanges)
                reactiveChange.ResetToCached();
        }

        protected ControlViewModel<T> GetBindedControl<T>(ReactiveProperty<T> modelProperty,
            bool isDelayed = false)
        {
            var controlViewModel = new ControlViewModel<T>(modelProperty, isDelayed)
                .AddTo(_disposables);
            
            _controlReactiveChanges.Add(controlViewModel);

            return controlViewModel;
        }

        protected ControlViewModel<TOwn, TModel> GetBindedControl<TOwn, TModel>(
            ReactiveProperty<TModel> modelProperty, Func<TModel, TOwn> getPipe, Func<TOwn, TModel> setPipe,
            bool isDelayed = false)
        {
            var controlViewModel = new ControlViewModel<TOwn, TModel>(modelProperty, getPipe, setPipe, isDelayed)
                .AddTo(_disposables);
            
            _controlReactiveChanges.Add(controlViewModel);

            return controlViewModel;
        }

        public virtual void Dispose()
        {
            _controlReactiveChanges.Clear();
            Disposes.ClearDispose(ref _disposables);
        }
    }
}