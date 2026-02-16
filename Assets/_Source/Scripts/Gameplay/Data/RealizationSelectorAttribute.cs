using System;
using UnityEngine;

namespace ITCafe.Data
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RealizationSelectorAttribute : PropertyAttribute
    {
        public Type ParentType { get; }

        public RealizationSelectorAttribute(Type parentType)
        {
            ParentType = parentType;
        }
    }
}