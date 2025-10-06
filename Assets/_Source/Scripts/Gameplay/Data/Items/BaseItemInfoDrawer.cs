#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using ITCafe.Data;
using ITCafe.Data.Items;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ITCafe.Editor
{
    [CustomPropertyDrawer(typeof(ItemTypeSelectorAttribute))]
    public class ItemTypeSelectorDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (!IsValidProperty(property))
            {
                return new Label("ItemTypeSelectorAttribute can only be used with BaseItemInfo fields");
            }

            var container = new VisualElement();
            
            var itemInfoTypes = GetItemInfoTypes();
            var typeNames = itemInfoTypes.Select(t => GetTypeDisplayName(t)).ToList();
            typeNames.Insert(0, "None");
            
            var dropdown = new PopupField<string>("Item Info Type", typeNames, 0);
            dropdown.tooltip = "Select the type of item info";
            dropdown.style.marginBottom = 5;
            
            var currentType = GetCurrentType(property);
            var currentIndex = currentType != null ? 
                typeNames.IndexOf(GetTypeDisplayName(currentType)) : 0;
            
            if (currentIndex >= 0)
            {
                dropdown.SetValueWithoutNotify(typeNames[currentIndex]);
            }
            
            dropdown.RegisterValueChangedCallback(evt =>
            {
                var selectedTypeName = evt.newValue;
                if (selectedTypeName == "None")
                {
                    property.managedReferenceValue = null;
                }
                else
                {
                    var selectedType = itemInfoTypes.FirstOrDefault(t => 
                        GetTypeDisplayName(t) == selectedTypeName);
                    
                    if (selectedType != null)
                    {
                        try
                        {
                            var newInstance = Activator.CreateInstance(selectedType);
                            property.managedReferenceValue = newInstance;
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Failed to create instance of {selectedType.Name}: {ex.Message}");
                        }
                    }
                }
                
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
            });
            
            container.Add(dropdown);
            
            var propertyField = new PropertyField(property);
            propertyField.BindProperty(property);
            container.Add(propertyField);
            
            return container;
        }
        
        private bool IsValidProperty(SerializedProperty property)
        {
            return property.type == "managedReference<BaseItemInfo>" || 
                   property.type.StartsWith("managedReference<") && 
                   property.type.Contains("BaseItemInfo");
        }
        
        private List<Type> GetItemInfoTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && 
                              !type.IsAbstract && 
                              typeof(BaseItemInfo).IsAssignableFrom(type))
                .OrderBy(type => type.Name)
                .ToList();
        }
        
        private Type GetCurrentType(SerializedProperty property)
        {
            if (property.managedReferenceValue != null)
            {
                return property.managedReferenceValue.GetType();
            }
            return null;
        }
        
        private string GetTypeDisplayName(Type type)
        {
            return type.Name;
        }
    }
}
#endif