using System;
using System.Collections.Generic;
using System.Linq;
using DevKit.UI.MVVM;
using R3;
using DevKit.Utils;
using ITCafe.Data.Settings;
using ObservableCollections;

namespace ITCafe.Gameplay.UI.MVVM
{
    public abstract class SettingsSectionViewModel : IViewModel, IDisposable
    {
        public ReadOnlyReactiveProperty<bool> IsAnyChanges = new ReactiveProperty<bool>(false);

        protected SettingsModel _model;
        protected ISettingsData _data;
        protected readonly List<IReactiveChange> _changeProperties = new();
        protected readonly List<Action> _cancelChanges = new();
        protected CompositeDisposable _disposables;
        protected ObservableHashSet<ISettingBarData> _settingWarnings;
        
        public void Bind(SettingsModel model)
        {
            _model = model;
            _disposables = new();

            OnBind(model);
            TrackChanges();
        }

        public void SetSettingsData(ISettingsData data, ObservableHashSet<ISettingBarData> settingWarnings)
        {
            _data = data;
            _settingWarnings = settingWarnings;
        }

        protected virtual void OnBind(SettingsModel model)
        {
        }

        private void TrackChanges()
        {
            IsAnyChanges = Observable.CombineLatest(_changeProperties.Select(x => x.IsChanged))
                .Select(x => x.Any(t => t))
                .ToReadOnlyReactiveProperty();
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

        protected SettingControlViewModel<T> CreateBindedProperty<T>(ReactiveProperty<T> modelProperty,
            bool isDelayed = false)
        {
            var controlViewModel = new SettingControlViewModel<T>(modelProperty, isDelayed)
                .AddTo(_disposables);
            
            _changeProperties.Add(controlViewModel);
            _cancelChanges.Add(() => controlViewModel.ResetToCached());

            return controlViewModel;
        }

        protected SettingControlViewModel<TOwn, TModel> CreateBindedProperty<TOwn, TModel>(
            ReactiveProperty<TModel> modelProperty, Func<TModel, TOwn> getPipe, Func<TOwn, TModel> setPipe,
            bool isDelayed = false)
        {
            var controlViewModel = new SettingControlViewModel<TOwn, TModel>(modelProperty, getPipe, setPipe, isDelayed)
                .AddTo(_disposables);
            
            _changeProperties.Add(controlViewModel);
            _cancelChanges.Add(() => controlViewModel.ResetToCached());

            return controlViewModel;
        }

        public virtual void Dispose()
        {
            Disposes.ClearDispose(ref _disposables);
        }
    }
}