using System;
using DevKit.Utils;
using R3;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class ControlViewModel<T> : ControlViewModel<T, T>
    {
        public ControlViewModel(ReactiveProperty<T> modelProperty, bool isDelayed = false) :
            base(modelProperty, isDelayed)
        {
            if (isDelayed)
            {
                _reactiveChange = new ReactiveChangeApplying<T>(() => modelProperty.Value, x => modelProperty.Value = x,
                    modelProperty.Value);
            }
            else
            {
                _reactiveChange = new ReactiveChange<T>(() => modelProperty.Value, modelProperty.Value);
            }

            OnChanged = _reactiveChange;
            _modelSubscription = modelProperty.Skip(1)
                .Subscribe(x => _reactiveChange.Value = x);
        }

        public override void SetValue(T value)
        {
            if (_isDelayed)
                _reactiveChange.Value = value;
            else
                _modelProperty.Value = value;
        }

        public override void ResetToCached()
        {
            _reactiveChange.ResetToCached();
            if (!_isDelayed)
                _modelProperty.Value = _reactiveChange.CachedValue;
        }
    }

    public class ControlViewModel<TOwn, TModel> : IControlViewModel<TOwn>, IReactiveChange, IDisposable
    {
        public ReadOnlyReactiveProperty<TOwn> OnChanged { get; protected set; }
        public ReadOnlyReactiveProperty<bool> IsChanged => _reactiveChange.IsChanged;
        public Observable<bool> OnWarning => _isWarning;

        protected ReactiveChangeBase<TOwn> _reactiveChange;
        protected readonly ReactiveProperty<TModel> _modelProperty;
        protected readonly bool _isDelayed;
        private readonly Func<TModel, TOwn> _getPipe;
        private readonly Func<TOwn, TModel> _setPipe;

        private readonly ReactiveProperty<bool> _isWarning = new(false);
        protected IDisposable _modelSubscription;

        public virtual void SetValue(TOwn value)
        {
            if (_isDelayed)
                _reactiveChange.Value = value;
            else
                _modelProperty.Value = _setPipe(value);
        }

        public virtual void SetWarning(bool isWarning)
        {
            _isWarning.Value = isWarning;
        }

        protected ControlViewModel(ReactiveProperty<TModel> modelProperty, bool isDelayed = false)
        {
            _modelProperty = modelProperty;
            _isDelayed = isDelayed;
        }

        public ControlViewModel(ReactiveProperty<TModel> modelProperty, Func<TModel, TOwn> getPipe,
            Func<TOwn, TModel> setPipe, bool isDelayed = false) : this(modelProperty, isDelayed)
        {
            _setPipe = setPipe;
            _getPipe = getPipe;

            Func<TOwn> getter = () => getPipe(modelProperty.Value);
            Action<TOwn> setter = x => modelProperty.Value = setPipe(x);

            if (isDelayed)
            {
                _reactiveChange = new ReactiveChangeApplying<TOwn>(getter, setter,
                    getPipe(modelProperty.Value));
            }
            else
            {
                _reactiveChange = new ReactiveChange<TOwn>(getter, getPipe(modelProperty.Value));
            }

            OnChanged = _reactiveChange;
            _modelSubscription = modelProperty.Skip(1)
                .Subscribe(x => _reactiveChange.Value = getPipe(x));
        }

        public void ApplyChanges()
        {
            _reactiveChange.ApplyChanges();
        }

        public virtual void ResetToCached()
        {
            _reactiveChange.ResetToCached();
            if (!_isDelayed)
                _modelProperty.Value = _setPipe(_reactiveChange.CachedValue);
        }

        public void UpdateCache()
        {
            _reactiveChange.UpdateCache();
        }

        public void Dispose()
        {
            _modelSubscription.Dispose();
        }
    }
}