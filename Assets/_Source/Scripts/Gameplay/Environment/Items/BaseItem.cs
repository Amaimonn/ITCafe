using UnityEngine;

namespace ITCafe
{
    public abstract class BaseItem : MonoBehaviour, IItem
    {
        public abstract bool CanTake();
        public abstract void Take();
        public abstract void Drop();

    }
}