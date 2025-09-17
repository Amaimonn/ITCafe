using UnityEngine;

namespace Flopin.Utils
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out var component))
                return component;
            else
                component = gameObject.AddComponent<T>();
            return component;
        }
    }
}