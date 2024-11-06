#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace PAT
{


    [CustomPropertyDrawer(typeof(GamePlayTag))]
    public class TagPropertyDrawer : PropertyDrawer
    {
        private Dictionary<string, List<GamePlayTag>> groupedTags;
        private bool[] categoryFoldouts;
        private string[] categoryNames;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Fetch the ScriptableObject that holds the folders and enum entries
            GamePlayTagHelper enumHelper = GetEnumHelper();

            if (enumHelper != null)
            {
                EditorGUI.BeginProperty(position, label, property);

                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                // Create a dropdown button to show the hierarchical menu
                if (EditorGUI.DropdownButton(position, new GUIContent(property.enumDisplayNames[property.enumValueIndex]), FocusType.Keyboard))
                {
                    GenericMenu menu = new GenericMenu();

                    // Loop through each folder and populate the dropdown with sub-hierarchy
                    foreach (var folder in enumHelper.folders)
                    {
                        foreach (var enumEntry in folder.enums)
                        {
                            string menuItemPath = $"{folder.name}/{enumEntry.name}";
                            int enumValue = enumEntry.id;

                            // Add each entry as a selectable option in the dropdown
                            menu.AddItem(new GUIContent(menuItemPath), enumValue == property.intValue, () =>
                            {
                                // Update the selected value when an item is chosen
                                property.intValue = enumValue;  // Only update the value, not the display name
                                property.serializedObject.ApplyModifiedProperties();
                            });
                        }
                    }

                    // Show the dropdown menu
                    menu.ShowAsContext();
                }

                EditorGUI.EndProperty();
            }
            else
            {
                // Fall back to default display if the helper is missing
                EditorGUI.PropertyField(position, property, label);
            }
        }

        private GamePlayTagHelper GetEnumHelper()
        {
            // Logic to get the ScriptableObject (could be loaded from Resources or assigned elsewhere)
            return GamePlayTagHelper.instance;
        }

        private string GetSelectedEnumName(SerializedProperty property, GamePlayTagHelper enumHelper)
        {
            // Find the enum entry that matches the current property value (ID)
            foreach (var folder in enumHelper.folders)
            {
                foreach (var enumEntry in folder.enums)
                {
                    if (enumEntry.id == property.intValue)
                    {
                        return enumEntry.name;  // Display the correct name
                    }
                }
            }
            return "None";  // Fallback if no matching entry is found
        }
    }
}
#endif