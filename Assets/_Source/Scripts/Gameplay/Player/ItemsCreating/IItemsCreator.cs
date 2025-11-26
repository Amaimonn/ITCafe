using ITCafe.Environment;
using UnityEngine;

namespace ITCafe.Player
{
    public interface IItemsCreator
    {
        public T Get<T>(string key) where T : MonoBehaviour, IItem;
    }
}