using UnityEngine;

namespace ITCafe
{
    public interface IItem : IInteractable
    {
        public Transform transform { get; }
        public Vector3 CenterOffset { get; }
        public void Drop();
        public void SetPhysicsEnabled(bool isEnabled);
    }
}