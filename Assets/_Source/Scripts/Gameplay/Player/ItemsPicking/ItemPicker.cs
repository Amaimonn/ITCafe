using System;
using DevKit.Utils;
using ITCafe.Environment;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace ITCafe.Player
{
    public class ItemPicker : MonoBehaviour, IItemPicker, IDisposable
    {
        public ReadOnlyReactiveProperty<IItem> OnItemChanged => _currentItem;
        public bool IsDroppingBlocked { get; set; } = false;
        public Transform HoldingPoint => _holdingPoint;

        [SerializeField] private Transform _holdingPoint;
        [SerializeField] private Transform _dropPoint;
        [SerializeField] private InputActionReference _dropAction;

        [Inject] private InputService _inputService;
        private PlayerContext _playerContext;
        
        private readonly ReactiveProperty<IItem> _currentItem = new();
        private readonly ReactiveProperty<ItemPickerState> _currentState = new();
        private bool _wasTakenThisFrame = false;

        private Action<InputAction.CallbackContext> _onDrop;
        private IDisposable _dropSubscription;
        private EmptyHandsState _emptyState;
        private IDisposable _itemReleaseSubscription;

        public void Init(PlayerContext playerContext)
        {
            _playerContext = playerContext;
            _emptyState = new EmptyHandsState(this, _playerContext);
            ChangeState(_emptyState);

            _itemReleaseSubscription = _currentItem.Skip(1) // for external item destroy invocation processing
                .Where(x => x == null)
                .Subscribe(_ => ChangeState(_emptyState));
        }

        public bool CanTake(IItem item)
        {
            return _currentState.Value?.CanTake(item) ?? false;
        }

        public void ChangeState(ItemPickerState newState)
        {
            if (newState == _currentState.Value)
                return;
            
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
        }

        public bool TryTake(IItem item)
        {
            if (item == null || !CanTake(item))
                return false;
            
            Take(item);
            return true;
        }

        public void TryDrop()
        {
            if (_currentItem.Value == null || _wasTakenThisFrame || IsDroppingBlocked)
                return;

            Drop();
        }

        public void Drop()
        {
            FLogger.Log<ItemPicker>("Dropping item");
            
            ChangeState(_emptyState);
            
            if (_currentItem.Value == null)
                return;
            
            _currentItem.Value.transform.parent = null;
            _currentItem.Value.transform.position = _dropPoint.position;
            _currentItem.Value.Drop();
            _currentItem.Value = null;
        }

        public void Release()
        {
            ChangeState(_emptyState);
            
            if (_currentItem.Value == null)
                return;
            
            _currentItem.Value.transform.parent = null;
            _currentItem.Value.transform.position = _dropPoint.position;
            _currentItem.Value = null;
        }

        private void OnDrop(InputAction.CallbackContext _)
        {
            TryDrop();
        }

        public void Dispose()
        {
            Disposes.ClearDispose(ref _itemReleaseSubscription);
        }
    }
}
