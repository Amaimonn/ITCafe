using ITCafe.Player;
using UnityEngine;

namespace ITCafe.Environment
{
    public interface IItem : IInteractable
    {
        public bool CanHandle(IItemHandler handler, PlayerContext context);
        public void Handle(IItemHandler handler, PlayerContext context);
        public Transform transform { get; }
        public Vector3 CenterOffset { get; }
        public void Drop();
        public void SetPhysicsEnabled(bool isEnabled);
    }
}