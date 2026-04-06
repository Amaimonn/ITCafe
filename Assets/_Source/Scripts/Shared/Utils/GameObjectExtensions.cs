using UnityEngine;

namespace ITCafe.Shared.Utils
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out var component))
                return component;
            
            component = gameObject.AddComponent<T>();
            
            return component;
        }
    }
}