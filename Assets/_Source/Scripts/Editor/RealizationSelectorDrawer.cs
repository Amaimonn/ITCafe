#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DevKit.Utils;
using ITCafe.Data;
using ITCafe.Data.Items;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ITCafe.Editor
{
    [CustomPropertyDrawer(typeof(RealizationSelectorAttribute), true)]
    public class RealizationSelectorDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            
            var itemInfoTypes = GetItemInfoTypes();
            var typeNames = itemInfoTypes.Select(t => t.Name).ToList();
            typeNames.Insert(0, "None");

            var dropdown = new PopupField<string>("Realization", typeNames, 0)
            {
                tooltip = "Select the type of item info"
            };

            var currentType = GetCurrentType(property);
            var currentIndex = currentType != null ? typeNames.IndexOf(currentType.Name) : 0;
            
            if (currentIndex >= 0)
                dropdown.value = typeNames[currentIndex];
            
            dropdown.RegisterValueChangedCallback(evt =>
            {
                var selectedTypeName = evt.newValue;
                if (selectedTypeName == "None")
                {
                    property.managedReferenceValue = null;
                }
                else
                {
                    var selectedType = itemInfoTypes.FirstOrDefault(t => t.Name == selectedTypeName);
                    if (selectedType != null)
                    {
                        var newInstance = Activator.CreateInstance(selectedType);
                        property.managedReferenceValue = newInstance;
                    }
                }
                
                property.serializedObject.ApplyModifiedProperties();
            });
            
            container.Add(dropdown);
            
            var propertyField = new PropertyField(property);
            propertyField.Bind(property.serializedObject);
            container.Add(propertyField);
            
            return container;
        }
        
        private List<Type> GetItemInfoTypes()
        {
            var searchType = fieldInfo.GetCustomAttribute<RealizationSelectorAttribute>().ParentType;
            
            // TODO: use Unity TypeTree mb
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && 
                              !type.IsAbstract && 
                              searchType.IsAssignableFrom(type))
                .ToList();
        }
        
        private Type GetCurrentType(SerializedProperty property)
        {
            if (property.managedReferenceValue != null)
                return property.managedReferenceValue.GetType();

            return null;
        }
    }
}
#endif