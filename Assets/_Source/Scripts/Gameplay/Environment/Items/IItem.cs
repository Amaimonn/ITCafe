using System;
using System.Collections.Generic;
using ITCafe.Data.Items;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public interface IItem : IInteractable, ICompositeComponent
    {
        public ItemTag Tag { get; }
        public bool CanBeHandled(IItemHandler handler, PlayerContext context);
        public void BecomeHandled(IItemHandler handler, PlayerContext context);
        public Transform transform { get; }
        public Vector3 CenterOffset { get; }
        public void Drop();

        public void OnTaken();

        public void SetPhysicsEnabled(bool isEnabled);

        protected Dictionary<Type, object> CachedComponentsMap { get; }

#region ICompositeComponent
        /// <summary>
        /// Used to get Components directly from the transform. If 'this' is T then 'this' is returned.
        /// 
        /// Caution №1: don`t use components addition/deletion/activation in Runtime,
        /// because references in the map won`t be updated by default.
        /// 
        /// Caution №2: don`t use with multiple components of type T on the transform,
        /// because only the first found one will be returned.
        /// </summary>
        bool ICompositeComponent.TryGetCachedComponent<T>(out T component)
        {
            if (transform == null)
            {
                component = default(T);
                return false;
            }

            if (CachedComponentsMap.TryGetValue(typeof(T), out var untypedComponent) && untypedComponent != null)
            {
                component = (T)untypedComponent;
                return true;
            }

            if (this is T)
            {
                component = (T)this;
                CachedComponentsMap[typeof(T)] = component;
                return true;
            }

            if (transform.TryGetComponent(out component))
            {
                CachedComponentsMap[typeof(T)] = component;
                return true;
            }

            component = default(T);
            CachedComponentsMap[typeof(T)] = null;

            return false;
        }
#endregion
    }
}