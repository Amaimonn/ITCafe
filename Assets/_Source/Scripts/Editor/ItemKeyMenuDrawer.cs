#if UNITY_EDITOR
using ITCafe.Gameplay.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ITCafe.Editor
{
    [CustomPropertyDrawer(typeof(ItemKeyMenuAttribute))]
    public class ItemKeyMenuDrawer : PropertyDrawer
    {
        private static Lazy<string[]> _cachedItemKeys = new Lazy<string[]>(LoadItemKeys);
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var itemKeys = _cachedItemKeys.Value;
            
            var container = new VisualElement();
            
            if (itemKeys.Length == 0)
            {
                // Fallback to text field with refresh button
                var fallbackContainer = new VisualElement();
                fallbackContainer.style.flexDirection = FlexDirection.Row;
                
                var textField = new TextField(property.displayName)
                {
                    value = property.stringValue,
                    style = { flexGrow = 1 }
                };
                
                var refreshButton = new Button(() => 
                {
                    _cachedItemKeys = new Lazy<string[]>(LoadItemKeys);
                    // Note: In UI Toolkit, we can't easily refresh the property drawer
                    // This would require a more complex solution
                })
                {
                    text = "Refresh",
                    style = { width = 70, marginLeft = 5 }
                };
                
                textField.RegisterValueChangedCallback(evt =>
                {
                    property.stringValue = evt.newValue;
                    property.serializedObject.ApplyModifiedProperties();
                });
                
                fallbackContainer.Add(textField);
                fallbackContainer.Add(refreshButton);
                container.Add(fallbackContainer);
                
                return container;
            }

            // Create dropdown with search functionality
            var dropdown = new DropdownField(property.displayName)
            {
                choices = new List<string>(itemKeys),
                value = property.stringValue
            };

            // If current value is not in choices, add it
            if (!string.IsNullOrEmpty(property.stringValue) && !dropdown.choices.Contains(property.stringValue))
            {
                dropdown.choices.Insert(0, property.stringValue);
            }

            dropdown.RegisterValueChangedCallback(evt =>
            {
                property.stringValue = evt.newValue;
                property.serializedObject.ApplyModifiedProperties();
            });

            // Add some styling
            dropdown.AddToClassList("item-key-dropdown");
            
            container.Add(dropdown);
            
            // Add label showing the count of available keys
            var infoLabel = new Label($"{itemKeys.Length} item keys available")
            {
                style = 
                {
                    fontSize = 10,
                    unityFontStyleAndWeight = FontStyle.Italic,
                    color = new StyleColor(Color.gray),
                    marginTop = 2,
                    marginLeft = 2
                }
            };
            container.Add(infoLabel);

            return container;
        }

        private static string[] LoadItemKeys()
        {
            try
            {
                var itemKeys = new HashSet<string>();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                
                foreach (var assembly in assemblies)
                {
                    if (IsSystemAssembly(assembly)) continue;

                    try
                    {
                        foreach (var type in assembly.GetTypes())
                        {
                            CollectItemKeysFromType(type, itemKeys);
                        }
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        continue;
                    }
                }

                return itemKeys.OrderBy(key => key).ToArray();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load ItemKeys: {ex.Message}");
                return new string[0];
            }
        }

        private static void CollectItemKeysFromType(Type type, HashSet<string> itemKeys)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | 
                                     BindingFlags.Static | BindingFlags.FlattenHierarchy;

            foreach (var field in type.GetFields(flags))
            {
                if ((field.IsLiteral || (field.IsStatic && field.IsInitOnly)) &&
                    field.FieldType == typeof(string) && 
                    field.GetCustomAttribute<ItemKeyAttribute>() != null)
                {
                    try
                    {
                        if (field.GetValue(null) is string value && !string.IsNullOrEmpty(value))
                        {
                            itemKeys.Add(value);
                        }
                    }
                    catch
                    {
                        // Ignore inaccessible fields
                    }
                }
            }
        }

        private static bool IsSystemAssembly(Assembly assembly)
        {
            var name = assembly.FullName;
            return name.StartsWith("System.") || name.StartsWith("UnityEngine") || 
                   name.StartsWith("UnityEditor") || name.StartsWith("mscorlib") ||
                   name.Contains("VisualScripting") || name.Contains("TestRunner");
        }
    }
}
#endif