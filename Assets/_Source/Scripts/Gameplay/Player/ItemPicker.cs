using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ITCafe
{
    public class ItemPicker : MonoBehaviour, IItemPicker
    {
        public ReadOnlyReactiveProperty<IItem> CurrentItem => _currentItem;
        public Observable<bool> IsHoldingItem => _isHoldingItem;

        [SerializeField] private Transform _holdingPoint;
        [SerializeField] private Transform _dropPoint;
        [SerializeField] private InputActionReference _dropAction;

        private readonly ReactiveProperty<bool> _isHoldingItem = new(false);
        private readonly ReactiveProperty<IItem> _currentItem = new();
        private bool _wasTakenThisFrame = false;

        private void OnEnable()
        {
            _dropAction.action.started += OnDrop;
        }

        private void OnDisable()
        {
            _dropAction.action.started -= OnDrop;
        }

        public bool CanTake()
        {
            return !_isHoldingItem.Value && _currentItem.Value == null;
        }

        public void Take(IItem item)
        {
            Debug.Log($"Taking item {item.transform.name}");
            _currentItem.Value = item;
            _currentItem.Value.transform.parent = _holdingPoint;
            _currentItem.Value.transform.localPosition = Vector3.zero;
            _isHoldingItem.Value = true;
            _wasTakenThisFrame = true;
            Observable.NextFrame().Subscribe(_ => _wasTakenThisFrame = false);
        }

        public bool TryTake(IItem item)
        {
            if (!CanTake() || item == null)
                return false;

            Take(item);
            return true;
        }

        public void TryDrop()
        {
            if (!_isHoldingItem.Value || _currentItem == null || _wasTakenThisFrame)
                return;
            
            Debug.Log("Dropping item");
            _currentItem.Value.transform.parent = null;
            _currentItem.Value.transform.position = _dropPoint.position;
            _currentItem.Value.Drop();
            _currentItem.Value = null;
            _isHoldingItem.Value = false;
        }

        private void OnDrop(InputAction.CallbackContext _)
        {
            TryDrop();
        }
    }
}