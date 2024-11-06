#if UNITY_EDITOR

using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PAT
{
    public class HitboxInspection: MonoBehaviour
    {

        [ContextMenu("Visual Hitboxes")]
        public void Visualize()
        {
            Hitbox[] boxes = GetComponentsInChildren<Hitbox>(true);
            List<FieldInfo> fieldInfos = new List<FieldInfo>();
            
            Component[] components = GetComponentsInChildren<Component>();

            // Iterate through each component
            foreach (Component component in components)
            {
                if(component == null){ continue;}
                // Get all fields of the component using reflection
                FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                // Iterate through the fields to find ones that match the targetType
                foreach (FieldInfo field in fields)
                {
                    if (field.FieldType == typeof(Hitbox))
                    {
                        fieldInfos.Add(field);
                        // Get the value of the field
                        object fieldValue = field.GetValue(component);

                        // Log the GameObject name, component, field name, and its value
                        Debug.Log($"GameObject: {component.gameObject.name}, Component: {component.GetType().Name}, Field: {field.Name}, Value: {fieldValue}");
                    }
                }
            }


        }
    }
}

#endif