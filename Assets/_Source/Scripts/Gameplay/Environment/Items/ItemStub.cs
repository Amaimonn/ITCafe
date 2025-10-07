using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public class ItemStub : IItem
    {
        public bool CanHandle(IItemHandler handler, PlayerContext context) => throw new System.NotImplementedException();
        public void Handle(IItemHandler handler, PlayerContext context) => throw new System.NotImplementedException();
        
        public static ItemStub Default = new();
        public Transform transform { get; }
        public Vector3 CenterOffset { get; }
        
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

        public void SetPhysicsEnabled(bool isEnabled)
        {
            throw new System.NotImplementedException();
        }
    }
}