using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ITCafe
{
    public class ItemPicker : MonoBehaviour
    {
        public Observable<bool> IsHoldingItem => _isHoldingItem;
        [SerializeField] private Transform _holdingPoint;
        [SerializeField] private Transform _dropPoint;
        [SerializeField] private InputActionReference _dropAction;

        private readonly ReactiveProperty<bool> _isHoldingItem = new(false);
        private IItem _currentItem;

        private void OnEnable()
        {
            _dropAction.action.started += OnDrop;
        }

        private void OnDisable()
        {
            _dropAction.action.started -= OnDrop;
        }


        public void TryPickUp(IItem item)
        {
            if (_isHoldingItem.Value || _currentItem != null || item == null || !item.CanTake())
                return;

            _currentItem = item;
            _currentItem.transform.parent = _holdingPoint;
            _currentItem.transform.localPosition = Vector3.zero;
            _currentItem.Take();
            _isHoldingItem.Value = true;
        }

        public void TryDrop()
        {
            if (!_isHoldingItem.Value || _currentItem == null)
                return;

            _currentItem.transform.parent = null;
            _currentItem.transform.position = _dropPoint.position;
            _currentItem.Drop();
            _currentItem = null;
            _isHoldingItem.Value = false;
        }

        private void OnDrop(InputAction.CallbackContext _)
        {
            TryDrop();
        }
    }
}
