using UnityEngine;

namespace ITCafe
{
    [RequireComponent(typeof(Collider))]
    public class ProductItem : BaseItem
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rigidbody;

        #region MonoBehaviour
        private void OnValidate()
        {
            if (_collider == null)
                _collider = GetComponent<Collider>();
        }
        #endregion

        public override void Drop()
        {
            _collider.enabled = true;
            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(Camera.main.transform.forward * 1.2f, ForceMode.Impulse); // TODO: optimize
        }

        public override void PickUp()
        {
            _collider.enabled = false;
            _rigidbody.useGravity = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
        }
    }
}