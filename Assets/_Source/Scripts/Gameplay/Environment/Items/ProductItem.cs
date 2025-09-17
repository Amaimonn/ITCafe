using UnityEngine;
using Flopin.Utils;

namespace ITCafe
{
    [RequireComponent(typeof(Collider))]
    public class ProductItem : BaseItem
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Camera _camera;

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

        public override bool CanTake()
        {
            return true;
        }

        public override void Drop()
        {
            _collider.enabled = true;
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(_camera.transform.forward * 1.2f, ForceMode.Impulse);
        }

        public override void Take()
        {
            _collider.enabled = false;
            _rigidbody.useGravity = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
        }
    }
}