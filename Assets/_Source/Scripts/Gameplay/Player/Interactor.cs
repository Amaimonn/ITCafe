using System;
using ITCafe.Environment;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace ITCafe.Player
{
    public class Interactor : MonoBehaviour
    {
        public Observable<IInteractable> CurrentTarget => _target;
        public Observable<bool> CanInteract => _canInteract;
        public Observable<IItem> OnItemInteracted => _onItemInteracted;

        [SerializeField] private InputActionReference _interactAction;
        [SerializeField] private float _interactDistance;
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _interactableLayers;

        private readonly Subject<IItem> _onItemInteracted = new();
        private readonly ReactiveProperty<bool> _canInteract = new(false);
        private readonly ReactiveProperty<IInteractable> _target = new();
        [Inject] private readonly PlayerContext _playerContext;
        [Inject] private readonly InputService _inputService;

        private Action<InputAction.CallbackContext> _onInteract;
        private IDisposable _interactSubscription;

        public void Init()
        {
            _onInteract = OnInteract; //_inputService.MediateAction(_interactAction, OnInteract);
            var inputEntry = new InputEntry(() => _interactAction.action.started += _onInteract,
                () => _interactAction.action.started -= _onInteract, 90);
            _interactSubscription = _inputService.MakeOrderedSub(HashCode.Combine(_interactAction.action, "started"),
                inputEntry);
        }

#region MonoBehaviour
        private void Start()
        {
            if (_camera == null)
                _camera = Camera.main;
        }
        
        private void Update()
        {
            FindInteractables();
        }

        private void OnDestroy()
        {
            _interactSubscription?.Dispose();
        }
#endregion

        private void OnInteract(InputAction.CallbackContext context)
        {
            InteractWithTarget();
            // _inputService.StopPropagating(_interactAction);
        }

        private void InteractWithTarget()
        {
            if (_target.Value != null)
            {
                _target.Value.Interact(_playerContext);
                if (_target.Value is IItem item)
                    _onItemInteracted.OnNext(item);
            }
        }

        private void FindInteractables()
        {
            var ray = new Ray(_camera.transform.position, _camera.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * _interactDistance, Color.red, 0.5f);

            if (Physics.Raycast(ray, out var hit, _interactDistance, _interactableLayers) &&
                hit.collider.TryGetComponent<IInteractable>(out var item))
            {
                var canInteract = item.CanInteract(_playerContext);
                _canInteract.Value = canInteract;

                if (_target.Value != item)
                {
                    if (canInteract)
                    {
                        ChangeFocus(item);
                    }
                    else
                        RemoveFocus();
                }
                else if (!canInteract)
                {
                    RemoveFocus();
                }
            }
            else
            {
                _canInteract.Value = false;
                RemoveFocus();
            }
        }

        private void RemoveFocus()
        {
            if (_target.Value != null)
            {
                _target.Value.UnFocus();
                _target.Value = null;
            }
        }

        private void ChangeFocus(IInteractable item)
        {
            _target.Value?.UnFocus();
            item.Focus();
            _target.Value = item;
        }
    }
}