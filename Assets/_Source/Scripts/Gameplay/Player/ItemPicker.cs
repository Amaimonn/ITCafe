using System;
using ITCafe.Environment;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace ITCafe.Player
{
    public class ItemPicker : MonoBehaviour, IItemPicker
    {
        public ReadOnlyReactiveProperty<IItem> CurrentItem => _currentItem;
        public Observable<bool> IsHoldingItem => _isHoldingItem;
        public bool IsDroppingBlocked { get; set; } = false;
        public Transform HoldingPoint => _holdingPoint;

        [SerializeField] private Transform _holdingPoint;
        [SerializeField] private Transform _dropPoint;
        [SerializeField] private InputActionReference _dropAction;

        [Inject] InputService _inputService;
        private readonly ReactiveProperty<bool> _isHoldingItem = new(false);
        private readonly ReactiveProperty<IItem> _currentItem = new();
        private readonly ReactiveProperty<ItemPickerState> _currentState = new();
        private bool _wasTakenThisFrame = false;

        private Action<InputAction.CallbackContext> _onDrop;
        private IDisposable _dropSubscription;
        
        private void Awake()
        {
            // Начинаем с пустых рук
            ChangeState(new EmptyHandsState(this));
        }
        
        private void OnEnable()
        {
            _onDrop = OnDrop;//_inputService.MediateAction(_dropAction, OnDrop);
            var inputEntry = new InputEntry(() => _dropAction.action.started += _onDrop,
                () => _dropAction.action.started -= _onDrop, 80);
            _dropSubscription = _inputService.MakeOrderedSub(HashCode.Combine(_dropAction.action, "started"),
                inputEntry);
        }

        private void OnDisable()
        {
            _dropSubscription.Dispose();
        }

        public bool CanTake(IItem item)
        {
            return _currentState.Value?.CanTake(item) ?? false;
            // return !_isHoldingItem.Value && _currentItem.Value == null;
        }

        public void ChangeState(ItemPickerState newState)
        {
            _currentState.Value?.OnExit();
            _currentState.Value = newState;
            _currentState.Value.OnEnter();
        }
        
        public void SetCurrentItem(IItem item)
        {
            _currentItem.Value = item;
        }

        public void Take(IItem item)
        {
            Debug.Log($"Taking item {item.transform.name}");
            _currentState.Value?.Take(item);
            _wasTakenThisFrame = true;
            Observable.NextFrame().Subscribe(_ => _wasTakenThisFrame = false);
            // item.transform.parent = _holdingPoint;
            // item.transform.SetLocalPositionAndRotation(-item.CenterOffset, Quaternion.identity);
            // _currentItem.Value = item;
            // _isHoldingItem.Value = true;
            // _wasTakenThisFrame = true;
            // Observable.NextFrame().Subscribe(_ => _wasTakenThisFrame = false);
        }

        public bool TryTake(IItem item)
        {
            if (!CanTake(item) || item == null)
                return false;
            
            Take(item);
            return true;
        }

        public void TryDrop()
        {
            if (_currentItem.Value == null || _wasTakenThisFrame || IsDroppingBlocked)
                return;

            Drop();
            // _inputService.StopPropagating(_dropAction);
        }

        public void Drop()
        {
            Debug.Log("Dropping item");
            _currentItem.Value.transform.parent = null;
            _currentItem.Value.transform.position = _dropPoint.position;
            _currentItem.Value.Drop();
            _currentItem.Value = null;
            _isHoldingItem.Value = false;
            ChangeState(new EmptyHandsState(this));
        }

        public void Release()
        {
            _currentState.Value = new EmptyHandsState(this);
            _currentItem.Value.transform.parent = null;
            _currentItem.Value.transform.position = _dropPoint.position;
            _currentItem.Value = null;
            _isHoldingItem.Value = false;
        }

        private void OnDrop(InputAction.CallbackContext _)
        {
            TryDrop();
        }
    }
}