using UnityEngine;

namespace ITCafe
{
    public class ProductItem : BaseItem
    {
        public override bool CanInteract(PlayerContext context)
        {
            return context.CurrentItem == null;
        }

        public override void Interact(PlayerContext context)
        {
            _collider.enabled = false;
            _rigidbody.useGravity = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            context.ItemPicker.TryPickUp(this);
        }
    }
}