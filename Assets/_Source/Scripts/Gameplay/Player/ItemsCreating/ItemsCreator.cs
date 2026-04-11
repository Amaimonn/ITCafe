using System;
using System.Collections.Generic;
using DevKit.Utils;
using ITCafe.Data.Items;
using ITCafe.Environment;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace ITCafe.Player
{
    public class ItemsCreator : IItemsCreator
    {
        private readonly Dictionary<ItemTag, GameObject> _keyedPrefabsMap = new();
        private readonly IObjectResolver _container;

        public ItemsCreator(IObjectResolver container)
        {
            _container = container;
        }
        
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
            {
                FLogger.LogError<ItemsCreator>($"{key} not found");
                return null;
            }
            
            var item = Object.Instantiate(itemPrefab).GetComponent<T>();
            
            _container.Inject(item);
            item.SetPhysicsEnabled(false);
                
            return item;
        }
        
        public IItem Get(ItemTag tag)
        {
            if (!_keyedPrefabsMap.TryGetValue(tag, out var itemPrefab))
            {
                FLogger.LogError<ItemsCreator>($"{tag} not found");
                return null;
            }

            var item = Object.Instantiate(itemPrefab).GetComponent<IItem>();
            
            _container.Inject(item);
            item.SetPhysicsEnabled(false);
                
            return item;
        }
    }
}