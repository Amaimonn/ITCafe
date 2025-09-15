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
        private BaseItem _currentItem;

        private void OnEnable()
        {
            _dropAction.action.started += OnDrop;
        }

        private void OnDisable()
        {
            _dropAction.action.started -= OnDrop;
        }


        public void TryPickUp(BaseItem item)
        {
            if (_isHoldingItem.Value || _currentItem != null)
                return;

            Debug.Log("Picked up " + item.name);
            _currentItem = item;
            _currentItem.transform.parent = _holdingPoint;
            _currentItem.transform.localPosition = Vector3.zero;
            _currentItem.PickUp();
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
