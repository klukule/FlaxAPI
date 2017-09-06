////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FlaxEditor.CustomEditors.Editors;
using FlaxEngine;

namespace FlaxEditor.CustomEditors
{
    internal static class CustomEditorsUtil
    {
        /// <summary>
        /// Gets the property name for UI. Removes unnecessary characters and filters text. Makes it more user-friendly.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The result.</returns>
        public static string GetPropertyNameUI(string name)
        {
            int length = name.Length;
            StringBuilder sb = new StringBuilder(length + 4);

            for (int i = 0; i < length; i++)
            {
                var c = name[i];

                if (char.IsUpper(c) && i > 0)
                {
                    if (i + 2 < length && !char.IsUpper(name[i + 1]) && !char.IsUpper(name[i + 2]))
                        sb.Append(' ');
                }
                else if (c == '_')
                {
                    if (sb.Length > 0)
                        sb.Append(' ');
                    continue;
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        internal static CustomEditor CreateEditor(ValueContainer values, CustomEditor overrideEditor, bool canUseRefPicker = true)
        {
            // Check if use provided editor
            if (overrideEditor != null)
                return overrideEditor;

            // Special case if property is a pure object type and all values are the same type
            if (values.Type == typeof(object) && values.Count > 0 && values[0] != null && !values.HasDiffrentTypes)
                return CreateEditor(values[0].GetType(), canUseRefPicker);

            // Use editor for the property type
            return CreateEditor(values.Type, canUseRefPicker);
        }

        internal static CustomEditor CreateEditor(Type targetType, bool canUseRefPicker = true)
        {
            if (targetType.IsArray)
            {
                return new ArrayEditor();
            }
            if (canUseRefPicker)
            {
                if (targetType.IsSubclassOf(typeof(Asset)))
                {
                    return new AssetRefEditor();
                }
                if (targetType.IsSubclassOf(typeof(FlaxEngine.Object)))
                {
                    return new FlaxObjectRefEditor();
                }
            }

            // Use custom editor
            {
                var checkType = targetType;
                do
                {
                    var type = Internal_GetCustomEditor(checkType);
                    if (type != null)
                    {
                        return (CustomEditor)Activator.CreateInstance(type);
                    }
                    checkType = checkType.BaseType;
                    
                    // Skip if cannot use ref editors
                    if (!canUseRefPicker && checkType == typeof(FlaxEngine.Object))
                        break;

                } while (checkType != null);
            }

            // Use attribute editor
            var attributes = targetType.GetCustomAttributes(false);
            var customEditorAttribute = (CustomEditorAttribute)attributes.FirstOrDefault(x => x is CustomEditorAttribute);
            if (customEditorAttribute != null)
                return (CustomEditor)Activator.CreateInstance(customEditorAttribute.Type);

            // Select default editor (based on type)
            if (targetType.IsEnum)
            {
                return new EnumEditor();
            }
            if (targetType.IsGenericType)
            {
                if (targetType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    return new ListEditor();
                }
                if (targetType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    // TODO: dedicated Dictionary<T> editor
                    return new GenericEditor();
                }
            }
            
            // The most generic editor
            return new GenericEditor();
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern Type Internal_GetCustomEditor(Type targetType);
    }
}
