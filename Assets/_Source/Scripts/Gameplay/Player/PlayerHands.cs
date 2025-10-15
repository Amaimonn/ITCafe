using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace ITCafe.Player
{
    public class PlayerHands : MonoBehaviour
    {
        [SerializeField] private InputActionReference _interactAction;
        [Space]
        [SerializeField] private Interactor _interactor;
        [SerializeField] private ItemPicker _itemPicker;

        [Inject] private readonly InputService _inputService;
        
        private Action<InputAction.CallbackContext> _onInteract;
        private IDisposable _interactSubscription;

        private void OnEnable()
        {
            _onInteract = OnInteract;
            var inputEntry = new InputEntry(
                () => _interactAction.action.started += _onInteract,
                () => _interactAction.action.started -= _onInteract, 
                90);
            _interactSubscription = _inputService.MakeOrderedSub(HashCode.Combine(_interactAction.action, "started"),
                inputEntry);
        }

        private void OnDisable()
        {
            _interactSubscription.Dispose();
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            if (!_interactor.Execute()) 
                _itemPicker.Execute();
        }
    }
}