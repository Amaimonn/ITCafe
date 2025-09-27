using UnityEngine;

namespace ITCafe.Environment
{
    public interface IItem : IInteractable
    {
        public Transform transform { get; }
        public Vector3 CenterOffset { get; }
        public void Drop();
        public void SetPhysicsEnabled(bool isEnabled);
    }
}