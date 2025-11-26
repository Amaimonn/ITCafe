using System;
using System.Collections.Generic;
using DevKit.Utils;
using ITCafe.Environment;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ITCafe.Player
{
    public class ItemsCreator : IItemsCreator
    {
        private readonly Dictionary<string, GameObject> _keyedPrefabsMap = new();

        public void Register(GameObject itemPrefab, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                FLogger.LogError<ItemsCreator>("Key is null or empty");
                return;
            }
            
            var type = itemPrefab.GetComponent<IItem>().GetType();
            if (!typeof(IItem).IsAssignableFrom(type))
                throw new ArgumentException("Prefab`s Type must be assignable to IItem", nameof(itemPrefab));

            _keyedPrefabsMap.Add(key, itemPrefab);
        }
        
        /// <summary>
        /// Get registered item by type with physics disabled.
        /// </summary>
        public T Get<T>(string key) where T : MonoBehaviour, IItem
        {
            if (!_keyedPrefabsMap.TryGetValue(key, out var itemPrefab))
                throw new KeyNotFoundException($"{key} not found");
            
            var item = Object.Instantiate(itemPrefab).GetComponent<T>();
            item.SetPhysicsEnabled(false);
                
            return item;
        }
    }
}