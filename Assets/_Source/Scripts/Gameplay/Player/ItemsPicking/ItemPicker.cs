using ITCafe.Environment;
using R3;
using UnityEngine;

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

        private readonly ReactiveProperty<bool> _isHoldingItem = new(false);
        private readonly ReactiveProperty<IItem> _currentItem = new();
        private readonly ReactiveProperty<ItemPickerState> _currentState = new();
        private bool _wasTakenThisFrame = false;

        private void Awake()
        {
            ChangeState(new EmptyHandsState(this));
        }
        
        public bool CanTake(IItem item)
        {
            return _currentState.Value?.CanTake(item) ?? false;
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
        }

        public bool TryTake(IItem item)
        {
            if (!CanTake(item) || item == null)
                return false;
            
            Take(item);
            return true;
        }

        public bool Execute() 
            => TryDrop();

        public bool TryDrop()
        {
            if (_currentItem.Value == null || _wasTakenThisFrame || IsDroppingBlocked)
                return false;

            Drop();
            return true;
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
    }
}