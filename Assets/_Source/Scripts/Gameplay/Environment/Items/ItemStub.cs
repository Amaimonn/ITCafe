using System;
using System.Collections.Generic;
using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public class ItemStub : IItem
    {
        public bool CanBeHandled(IItemHandler handler, PlayerContext context) => throw new System.NotImplementedException();
        public void BecomeHandled(IItemHandler handler, PlayerContext context) => throw new System.NotImplementedException();
        
        public static ItemStub Default = new();
        public Transform transform { get; }
        public Vector3 CenterOffset { get; }

        Dictionary<Type, object> IItem.CachedComponentsMap => null;

        public void Focus()
        {
            throw new System.NotImplementedException();
        }

        public void UnFocus()
        {
            throw new System.NotImplementedException();
        }

        public bool CanInteract(PlayerContext context)
        {
            throw new System.NotImplementedException();
        }

        public void Interact(PlayerContext context)
        {
            throw new System.NotImplementedException();
        }

        public void Drop()
        {
            throw new System.NotImplementedException();
        }
        
        public void OnTaken()
        {
            throw new System.NotImplementedException();
        }

        public void SetPhysicsEnabled(bool isEnabled)
        {
            throw new System.NotImplementedException();
        }
    }
}