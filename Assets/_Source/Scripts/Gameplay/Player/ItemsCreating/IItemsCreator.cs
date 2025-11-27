using System;
using ITCafe.Data.Items;
using ITCafe.Environment;
using UnityEngine;

namespace ITCafe.Player
{
    public interface IItemsCreator
    {
        public T Get<T>(ItemTag key) where T : MonoBehaviour, IItem;
        public IItem Get(ItemTag key);
    }
}