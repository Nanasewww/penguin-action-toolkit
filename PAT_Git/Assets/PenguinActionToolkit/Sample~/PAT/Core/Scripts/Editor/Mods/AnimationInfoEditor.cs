using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.Collections.Generic;

namespace PAT
{
    [CustomPropertyDrawer(typeof(AnimationInfo))]
    public class AnimationInfoEditor : PropertyDrawer
    {
        private string[] layerNames;
        private string[] stateNames;
        private AnimationClip selectedClip;
        
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            
            
            // Start property field with foldout
            EditorGUI.BeginProperty(position, label, property);

            // Fetch the properties
            var animatorControllerProperty = property.FindPropertyRelative("referenceAnimator");
            var selectedLayerProperty = property.FindPropertyRelative("layer");
            var selectedStateProperty = property.FindPropertyRelative("stateName");
            var selectedFadeinProperty = property.FindPropertyRelative("fadeTime");
            var selectedFadeoutProperty = property.FindPropertyRelative("fadeOutTime");

            // Calculate positions for fields
            Rect animatorFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect layerFieldRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
            Rect stateFieldRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2) * 2, position.width, EditorGUIUtility.singleLineHeight);
            Rect fadeInFieldRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2) * 3, position.width, EditorGUIUtility.singleLineHeight);
            Rect fadeOutFieldRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2) * 4, position.width, EditorGUIUtility.singleLineHeight);
            Rect clipLabelRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2) * 5, position.width * 0.3f, EditorGUIUtility.singleLineHeight);
            Rect clipButtonRect = new Rect(position.x + position.width * 0.3f + 5, position.y + (EditorGUIUtility.singleLineHeight + 2) * 5, position.width * 0.7f - 5, EditorGUIUtility.singleLineHeight);
            
            // Custom Button Style
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white },
                hover = { textColor = Color.cyan },
                fontSize = 12
            };
            
            // Animator Controller Field
            EditorGUI.PropertyField(animatorFieldRect, animatorControllerProperty, new GUIContent("Reference Animation Controller"));

            // If multiple objects are being edited, show a warning
            if (property.serializedObject.isEditingMultipleObjects)
            {
                EditorGUI.LabelField(position, "Multi-object editing is not supported.");
                return;
            }            
            
            // If an AnimatorController is assigned, populate the dropdowns
            if (animatorControllerProperty.objectReferenceValue != null)
            {
                var animatorController = animatorControllerProperty.objectReferenceValue as  AnimatorController;

                if (animatorController != null)
                {
                    PopulateLayerNames(animatorController);
                    PopulateStateNames(animatorController, selectedLayerProperty.intValue);

                    // Layer Dropdown
                    if (selectedLayerProperty.intValue >= layerNames.Length)
                    {
                        selectedLayerProperty.intValue = 0;
                        Debug.LogWarning("Layer index out of bound, set to 0");
                    }
                    selectedLayerProperty.intValue = EditorGUI.Popup(layerFieldRect, "Layer", selectedLayerProperty.intValue, layerNames);

                    // State Dropdown
                    string stateName = selectedStateProperty.stringValue;
                    int stateIndex = GetStateIndexFromName( selectedLayerProperty.intValue, stateName);
                    selectedStateProperty.stringValue =
                        stateNames[EditorGUI.Popup(stateFieldRect, "Animation State", stateIndex, stateNames)];
                    
                    EditorGUI.PropertyField(fadeInFieldRect, selectedFadeinProperty, new GUIContent("Fade in Time"));
                    EditorGUI.PropertyField(fadeOutFieldRect, selectedFadeoutProperty, new GUIContent("Fade out Time"));
                    
                    // Retrieve the selected animation clip from the state
                    selectedClip = GetClipFromState(animatorController, selectedLayerProperty.intValue, selectedStateProperty.stringValue);

                    // Display the Animation Clip with a label
                    EditorGUI.LabelField(clipLabelRect, "Animation Clip:");
                
                    if (selectedClip != null)
                    {
                        // Make the button clickable
                        if (GUI.Button(clipButtonRect, selectedClip.name, buttonStyle))
                        {
                            // Ping (highlight) the animation clip in the project window
                            EditorGUIUtility.PingObject(selectedClip);
                        }
                    }
                    else
                    {
                        EditorGUI.LabelField(clipButtonRect, "No Animation Clip Assigned");
                    }
                    
                }
            }
            else
            {
                EditorGUI.IntField(layerFieldRect,"Layer", selectedLayerProperty.intValue);
                EditorGUI.PropertyField(stateFieldRect, selectedStateProperty, new GUIContent("Animation"));
                EditorGUI.PropertyField(fadeInFieldRect, selectedFadeinProperty, new GUIContent("Fade in Time"));
                EditorGUI.PropertyField(fadeOutFieldRect, selectedFadeoutProperty, new GUIContent("Fade out Time"));
                EditorGUI.LabelField(clipButtonRect, "No Animator Assigned as reference");
            }

            
            
            // End property field
            EditorGUI.EndProperty();
        }

        private void PopulateLayerNames(AnimatorController animatorController)
        {
            // Fetching layer names from AnimatorController
            layerNames = new string[animatorController.layers.Length];
            for (int i = 0; i < animatorController.layers.Length; i++)
            {
                layerNames[i] = animatorController.layers[i].name;
            }
        }

        private bool PopulateStateNames(AnimatorController animatorController, int layerIndex)
        {
            if (layerIndex >= animatorController.layers.Length)
            {
                layerIndex = 0;
                Debug.LogWarning("Layer index out of bound, showing layer 0");
            }
            
            // Fetching state names from the selected layer
            ChildAnimatorState[] states = animatorController.layers[layerIndex].stateMachine.states;
            List<string> stateList = new List<string>();
            foreach (var state in states)
            {
                stateList.Add(state.state.name);
            }
            stateNames = stateList.ToArray();
            return true;
        }
        
        private int GetStateIndexFromName(int layerIndex, string stateName)
        {
            for (int i = 0; i < stateNames.Length; i++)
            {
                if (stateNames[i] == stateName)
                {
                    return i;
                }
            }
            Debug.LogWarning("state not found, set to 0");
            return 0;  // Default to the first state if name is not found
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Each field takes one line
            return EditorGUIUtility.singleLineHeight * 6 + 15;
        }
        
        private AnimationClip GetClipFromState(AnimatorController animatorController, int layerIndex, string stateName)
        {
            var states = animatorController.layers[layerIndex].stateMachine.states;
            foreach (var state in states)
            {
                if (state.state.name == stateName)
                {
                    return state.state.motion as AnimationClip;
                }
            }
            return null;
        }
        
        
    }
}
