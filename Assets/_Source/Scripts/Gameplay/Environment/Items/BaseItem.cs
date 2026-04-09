using System;
using System.Collections.Generic;
using ITCafe.Shared.Utils;
using ITCafe.Shared;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    [RequireComponent(typeof(Collider))]
    public abstract class BaseItem : BaseInteractable, IItem
    {
        public virtual bool CanBeHandled(IItemHandler handler, PlayerContext context) =>
            handler.CanHandle(this, context);

        public virtual void BecomeHandled(IItemHandler handler, PlayerContext context) => handler.Handle(this, context);
        
        public Vector3 CenterOffset { get; private set; }

        [SerializeField] protected Collider _collider;
        [SerializeField] protected Rigidbody _rigidbody;
        [SerializeField] protected Camera _camera;
        
        Dictionary<Type, object> IItem.CachedComponentsMap { get; } = new();

#region MonoBehaviour
        protected override void OnValidate()
        {
            base.OnValidate();

            if (_collider == null)
                _collider = GetComponent<Collider>();

            if (_rigidbody == null)
                _rigidbody = gameObject.GetOrAddComponent<Rigidbody>();

            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            if (_camera == null)
                _camera = Camera.main;
        }

        protected override void Awake()
        {
            base.Awake();
            if (!TryGetComponent<Renderer>(out var renderer))
                renderer = GetComponentInChildren<Renderer>();

            CenterOffset = transform.InverseTransformPoint(renderer.bounds.center);
        }
#endregion
        
        public virtual void Drop()
        {
            SetPhysicsEnabled(true);
            _rigidbody.AddForce(_camera.transform.forward * 1.2f, ForceMode.Impulse);
        }

        public virtual void OnTaken()
        {
        }

        public void SetPhysicsEnabled(bool isEnabled)
        {
            if (_collider.enabled == isEnabled)
                return;

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