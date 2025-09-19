using UnityEngine;

namespace ITCafe
{
    public interface IItem : IInteractable
    {
        public Transform transform { get; }
        public void Drop();
    }
}