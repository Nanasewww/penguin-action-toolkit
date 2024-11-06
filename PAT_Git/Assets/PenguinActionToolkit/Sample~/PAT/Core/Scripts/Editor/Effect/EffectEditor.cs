using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PAT
{
    [CustomEditor(typeof(Effect))]
    [CanEditMultipleObjects]  // Enable multi-object editing
    public class ExampleObjectEditor : Editor
    {
        private List<Type> effectComponentTypes; // List to store the types
        private string[] effectComponentNames;   // Array for the dropdown
        private int selectedIndex = 0;           // Track the current selected index

        void OnEnable()
        {
            // Find all types that inherit from EffectComponent
            effectComponentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(EffectComponent).IsAssignableFrom(type) && !type.IsAbstract)
                .ToList();

            // Create an array of type names for the dropdown
            effectComponentNames = effectComponentTypes.Select(type => type.Name).ToArray();
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            // Get all selected EffectObjects
            Effect[] effectObjects = targets.Cast<Effect>().ToArray();

            // Display dropdown to select EffectComponent types
            selectedIndex = EditorGUILayout.Popup("Effect Component", selectedIndex, effectComponentNames);

            if (GUILayout.Button("Add Effect Component"))
            {
                Type selectedType = effectComponentTypes[selectedIndex];
                foreach (Effect effectObject in effectObjects)
                {
                    // Create a new instance of the selected EffectComponent
                    EffectComponent newComponent = ScriptableObject.CreateInstance(selectedType) as EffectComponent;
                    newComponent.name = selectedType.Name; // Optional: name the component instance

                    // Save the new component as an asset (or handle it as needed)
                    AssetDatabase.AddObjectToAsset(newComponent, effectObject);
                    effectObject.components.Add(newComponent);

                    // Mark the EffectObject as dirty so the changes will be saved
                    EditorUtility.SetDirty(effectObject);
                    AssetDatabase.SaveAssets();
                }
            }

            // Display and edit EffectComponents in the list
            for (int i = 0; i < effectObjects[0].components.Count; i++)
            {
                EffectComponent component = effectObjects[0].components[i];

                if (component == null) continue;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(component.GetType().Name, EditorStyles.boldLabel);

                // Create an editor for the EffectComponent
                Editor editor = Editor.CreateEditor(component);

                if (editor != null)
                {
                    // Draw default inspector for the component (shows all its serialized fields)
                    editor.OnInspectorGUI();
                }

                // Remove button
                if (GUILayout.Button("Remove"))
                {
                    foreach (Effect effectObject in effectObjects)
                    {
                        EffectComponent componentToRemove = effectObject.components[i];
                        effectObject.components.RemoveAt(i);

                        // Destroy the ScriptableObject instance
                        DestroyImmediate(componentToRemove, true);
                        AssetDatabase.SaveAssets();
                        EditorUtility.SetDirty(effectObject);
                    }
                }
                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}