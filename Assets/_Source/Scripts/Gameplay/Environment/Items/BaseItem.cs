using Flopin.Utils;
using UnityEngine;

namespace ITCafe
{
    [RequireComponent(typeof(Collider))]
    public abstract class BaseItem : MonoBehaviour, IItem
    {
        [SerializeField] protected Collider _collider;
        [SerializeField] protected Rigidbody _rigidbody;
        [SerializeField] protected Camera _camera;

        #region MonoBehaviour
        private void OnValidate()
        {
            if (_collider == null)
                _collider = GetComponent<Collider>();

            if (_rigidbody == null)
                _rigidbody = gameObject.GetOrAddComponent<Rigidbody>();

            if (_camera == null)
                _camera = Camera.main;
        }
        #endregion

        public virtual void Focus()
        {
            Debug.Log($"Focus: {name}");
        }

        public virtual void UnFocus()
        {
            Debug.Log($"Unfocus: {name}");
        }

        public abstract bool CanInteract(PlayerContext context);
        public abstract void Interact(PlayerContext context);

        public virtual void Drop()
        {
            _collider.enabled = true;
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(_camera.transform.forward * 1.2f, ForceMode.Impulse);
        }
    }
}