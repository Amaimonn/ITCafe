using System;
using System.Collections.Generic;
using DevKit.UI.MVVM;
using R3;
using DevKit.Utils;
using ITCafe.Data;

namespace ITCafe.Gameplay.UI.MVVM
{
    public abstract class SettingsSectionViewModel : IViewModel, IDisposable
    {
        public ReadOnlyReactiveProperty<bool> IsAnyChanges = new ReactiveProperty<bool>(false);

        protected SettingsModel _model;
        protected CompositeDisposable _disposables;
        protected List<IReactiveChange> _changeProperties = new();
        protected List<Action> _cancelChanges = new();
        
        public void Bind(SettingsModel model)
        {
            _model = model;
            _disposables = new();
            OnBind(model);
        }

        protected virtual void OnBind(SettingsModel model)
        {
            
        }

        public virtual void ApplyChanges()
        {
            foreach (var change in _changeProperties)
                change.ApplyChanges();
        }

        public virtual void CancelChanges()
        {
            foreach (var cancel in _cancelChanges)
                cancel();
        }

        protected ReactiveChange<T> CreateBindedProperty<T>(ReactiveProperty<T> modelProperty)
        {
            var viewModelProperty = new ReactiveChange<T>(() => modelProperty.Value, modelProperty.Value);

            BindChanges(modelProperty, viewModelProperty);
            _changeProperties.Add(viewModelProperty);
            _cancelChanges.Add(() =>
            {
                viewModelProperty.ResetToCached();
                modelProperty.Value = viewModelProperty.CachedValue;
            });

            return viewModelProperty;
        }

        protected ReactiveChangeApplying<T> CreateBindedPropertyApplying<T>(ReactiveProperty<T> modelProperty)
        {
            var viewModelProperty = new ReactiveChangeApplying<T>(() => modelProperty.Value, x => modelProperty.Value = x,
                modelProperty.Value);

            BindChanges(modelProperty, viewModelProperty);
            _changeProperties.Add(viewModelProperty);
            _cancelChanges.Add(() => viewModelProperty.ResetToCached());

            return viewModelProperty;
        }

        private void BindChanges<T>(Observable<T> modelProperty, ReactiveChangeBase<T> viewModelProperty)
        {
            modelProperty.Skip(1).Subscribe(x => viewModelProperty.Value = x)
                .AddTo(_disposables);
        }

        public virtual void Dispose()
        {
            Disposes.ClearDispose(ref _disposables);
        }
    }
}
