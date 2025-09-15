using UnityEngine;

namespace ITCafe
{
    public abstract class BaseItem : MonoBehaviour, IItem
    {
        public abstract void PickUp();
        public abstract void Drop();
    }
}