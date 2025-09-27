using Flopin.Utils;
using UnityEngine;

namespace ITCafe
{
    [RequireComponent(typeof(Collider))]
    public abstract class BaseItem : BaseInteractable, IItem
    {
        public Vector3 CenterOffset => _centerOffset;
        [SerializeField] protected Collider _collider;
        [SerializeField] protected Rigidbody _rigidbody;
        [SerializeField] protected Camera _camera;
        private Vector3 _centerOffset;

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

        protected override void Awake()
        {
            base.Awake();
            _centerOffset = transform.InverseTransformPoint(GetComponent<Renderer>().bounds.center);
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