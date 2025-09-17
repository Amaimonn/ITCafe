using UnityEngine;

namespace ITCafe
{
    public interface IItem
    {
        public Transform transform { get; }
        public bool CanTake();
        public void Take();
        public void Drop();
    }
}