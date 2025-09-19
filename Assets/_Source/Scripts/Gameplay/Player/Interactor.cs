using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace ITCafe
{
    public class Interactor : MonoBehaviour
    {
        public Observable<IItem> OnItemInteracted => _onItemInteracted;
        [SerializeField] private InputActionReference _interactAction;
        [SerializeField] private float _interactDistance;
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _interactableLayers;

        private readonly Subject<IItem> _onItemInteracted = new();
        private IItem _targetedItem;
        [Inject] private readonly PlayerContext _playerContext;

        #region MonoBehaviour
        private void Start()
        {
            if (_camera == null)
                _camera = Camera.main;
        }

        private void OnEnable()
        {
            _interactAction.action.started += OnInteract;
        }

        private void OnDisable()
        {
            _interactAction.action.started -= OnInteract;
        }

        private void Update()
        {
            FindInteractables();
        }
        #endregion

        private void OnInteract(InputAction.CallbackContext context)
        {
            InteractWithTarget();
        }

        private void InteractWithTarget()
        {
            if (_targetedItem != null)
            {
                _targetedItem.Interact(_playerContext);
                _onItemInteracted.OnNext(_targetedItem);
            }
        }

        private void FindInteractables()
        {
            var ray = new Ray(_camera.transform.position, _camera.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * _interactDistance, Color.red, 0.5f);

            if (Physics.Raycast(ray, out var hit, _interactDistance, _interactableLayers) &&
                hit.collider.TryGetComponent<IItem>(out var item))
            {
                if (_targetedItem != item)
                {
                    if (item.CanInteract(_playerContext))
                    {
                        ChangeFocus(item);
                    }
                    else
                        RemoveFocus();
                }
            }
            else
            {
                RemoveFocus();
            }
        }

        private void RemoveFocus()
        {
            if (_targetedItem != null)
            {
                _targetedItem.UnFocus();
                _targetedItem = null;
            }
        }

        private void ChangeFocus(IItem item)
        {
            _targetedItem?.UnFocus();
            item.Focus();
            _targetedItem = item;
        }
    }
}