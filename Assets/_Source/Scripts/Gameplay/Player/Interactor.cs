using R3;
using UnityEngine;
using UnityEngine.InputSystem;

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
        #endregion

        private void OnInteract(InputAction.CallbackContext context)
        {
            TryInteract();
        }

        private void TryInteract()
        {
            var ray = new Ray(_camera.transform.position, _camera.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * _interactDistance, Color.red, 0.5f);
            
            if (Physics.Raycast(ray, out var hit, _interactDistance, _interactableLayers))
            {
                Debug.Log("Hit");
                if (hit.transform.gameObject.TryGetComponent<IItem>(out var item))
                {
                    Debug.Log("Interacted");
                    _onItemInteracted.OnNext(item);
                }
                else
                {
                    Debug.Log($"{hit.transform.gameObject.name} Not an item");
                }
            }
            else
            {
                Debug.Log("No hit");
            }
        }
    }
}