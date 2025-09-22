using Flopin.Utils;
using UnityEngine;

namespace ITCafe
{
    [RequireComponent(typeof(Collider))]
    public abstract class BaseItem : BaseInteractable, IItem
    {
        [SerializeField] protected Collider _collider;
        [SerializeField] protected Rigidbody _rigidbody;
        [SerializeField] protected Camera _camera;

        #region MonoBehaviour

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_collider == null)
                _collider = GetComponent<Collider>();

            if (_rigidbody == null)
                _rigidbody = gameObject.GetOrAddComponent<Rigidbody>();

            if (_camera == null)
                _camera = Camera.main;
        }

        #endregion

        public virtual void Drop()
        {
            SetPhysicsEnabled(true);
            _rigidbody.AddForce(_camera.transform.forward * 1.2f, ForceMode.Impulse);
        }

        public void SetPhysicsEnabled(bool isEnabled)
        {
            if (isEnabled)
            {
                _collider.enabled = true;
                _rigidbody.useGravity = true;
                _rigidbody.isKinematic = false;
            }
            else
            {
                _collider.enabled = false;
                _rigidbody.useGravity = false;
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                _rigidbody.isKinematic = true;
            }
        }
    }
}