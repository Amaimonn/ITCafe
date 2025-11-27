using System;
using System.Collections.Generic;
using ITCafe.Data.Items;
using ITCafe.Environment;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ITCafe.Player
{
    public class ItemsCreator : IItemsCreator
    {
        private readonly Dictionary<ItemTag, GameObject> _keyedPrefabsMap = new();

        public void Register(GameObject itemPrefab, ItemTag key)
        {
            var type = itemPrefab.GetComponent<IItem>().GetType();
            if (!typeof(IItem).IsAssignableFrom(type))
                throw new ArgumentException("Prefab`s Type must be assignable to IItem", nameof(itemPrefab));

            _keyedPrefabsMap.Add(key, itemPrefab);
        }
        
        /// <summary>
        /// Get registered item by type with physics disabled.
        /// </summary>
        public T Get<T>(ItemTag key) where T : MonoBehaviour, IItem
        {
            if (!_keyedPrefabsMap.TryGetValue(key, out var itemPrefab))
                throw new KeyNotFoundException($"{key} not found");
            
            var item = Object.Instantiate(itemPrefab).GetComponent<T>();
            item.SetPhysicsEnabled(false);
                
            return item;
        }
        
        public IItem Get(ItemTag tag)
        {
            if (!_keyedPrefabsMap.TryGetValue(tag, out var itemPrefab))
                throw new KeyNotFoundException($"{tag} not found");
            
            var item = Object.Instantiate(itemPrefab).GetComponent<IItem>();
            item.SetPhysicsEnabled(false);
                
            return item;
        }
    }
}