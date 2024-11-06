using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
#if UNITY_EDITOR
namespace PAT
{
    public class PAT_ActionNode : PAT_Node
    {
        //public List<PAT_ModifierNode> children = new List<PAT_ModifierNode>();
        //public MonoScript actionScript;
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(PAT_ActionNode))]
    public class PAT_ActionNodeInspector : Editor
    {
        // Start is called before the first frame update
        private PAT_ActionNode node;
        private List<MonoScript> previousScripts;

        SerializedProperty attachedScriptsProp;
         private Vector2 scrollPosition;
        private void OnEnable()
        {
            node = (PAT_ActionNode)target;

            // Store the initial state of the attached scripts to compare later
            if(node.attachedScripts is not null){

                previousScripts = new List<MonoScript>(node.attachedScripts);
            }
            attachedScriptsProp = serializedObject.FindProperty("attachedScripts");
        }

        public override void OnInspectorGUI()
        {

            serializedObject.Update();


            // EditorGUILayout.Space();
            // EditorGUILayout.LabelField("PAT Character Node Inspector", EditorStyles.boldLabel);
            // EditorGUILayout.Space();

            // Group the attached scripts display in a box with padding
            //EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Attached Scripts", EditorStyles.boldLabel);
            
            // Display the current list without letting users remove items
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(600));
            if (node.attachedScripts is null || node.attachedScripts.Count == 0)
            {
                EditorGUILayout.HelpBox("No scripts attached yet.", MessageType.Info);
            }
            else
            {
                for (int i = 0; i < node.attachedScripts.Count; i++)
                {
                    if (node.attachedScripts[i] != null)
                    {
                        // Draw a label for each script with a right-click context menu
                        DrawScriptEntryWithContextMenu(i);
                    }
                }
            }
            //EditorGUILayout.EndVertical(); // End the scripts list box

            EditorGUILayout.Space();
            EditorGUILayout.EndScrollView();

            if(node.attachedScripts is null){
                node.attachedScripts = new List<MonoScript>();
                previousScripts = new List<MonoScript>(node.attachedScripts);
            }
            // Check if the attachedScripts list has changed
            //EditorGUILayout.PropertyField(attachedScriptsProp, new GUIContent("Attached Scripts"), true);

        
            if (serializedObject.ApplyModifiedProperties() && EditorUtility.InstanceIDToObject(node.objid) is not null && node.isInitialized == true )
            {
                OnAttachedScriptsChanged();
                previousScripts = new List<MonoScript>(node.attachedScripts);
            }

            
            //serializedObject.Update();
            
            
        }

        private void DrawScriptEntryWithContextMenu(int index)
        {
            MonoScript script = node.attachedScripts[index];

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20); // Indentation

            GUIStyle scriptStyle = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 14, // Increase the font size
                padding = new RectOffset(10, 10, 10, 10), // Add some padding around the text
                fixedHeight = 40, // Set a larger fixed height for the item
                alignment = TextAnchor.MiddleLeft // Align the text to the left, vertically centered
            };
            // Detect right-click on the script label
            Rect scriptRect = EditorGUILayout.GetControlRect(false, 40);
            EditorGUI.LabelField(scriptRect, script.name, scriptStyle);

            if (Event.current.type == EventType.ContextClick && scriptRect.Contains(Event.current.mousePosition))
            {
                // Create a context menu when right-clicked
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Remove Script"), false, () => RemoveScriptAtIndex(index));
                menu.ShowAsContext();
                Event.current.Use();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void RemoveScriptAtIndex(int index)
        {
            // Remove the script at the specified index
            if (index >= 0 && index < node.attachedScripts.Count)
            {
                MonoScript removedScript = node.attachedScripts[index];
                node.attachedScripts.RemoveAt(index);
                //Debug.Log($"Removed Script: {removedScript.name}");

                // Optionally remove the component from the GameObject as well
                // GameObject nodeObj = (GameObject)EditorUtility.InstanceIDToObject(node.objid);
                // Component[] allComponent = nodeObj.GetComponents<Component>();
                // var type = removedScript.GetClass();
                // if (nodeObj != null && type != null)
                // {
                //     DestroyImmediate(allComponent.ToList()[index + 2]  );
                // }

                // Mark the object as dirty so the changes are saved
                EditorUtility.SetDirty(node);
            }
        }
        private void OnAttachedScriptsChanged()
        {
        
            GameObject nodeObj = (GameObject)EditorUtility.InstanceIDToObject(node.objid);
            // Here you can define what happens when the attachedScripts list is changed
            if (node.attachedScripts.Count > previousScripts.Count)
            {
                // Detect addition
                MonoScript addedScript = node.attachedScripts.Except(previousScripts).FirstOrDefault();
                if (addedScript != null){
                    //Debug.Log($"Script Added: {addedScript.name}");
                    nodeObj.AddComponent(addedScript.GetType());
                    serializedObject.Update();
                }
            }
            // else if (node.attachedScripts.Count < previousScripts.Count)
            // {
                
            //     // Detect removal
            //     MonoScript removedScript = previousScripts.Except(node.attachedScripts).FirstOrDefault();
            //     if (removedScript != null){
            //         Debug.Log($"Script Remove: {removedScript.name}");
            //         DestroyImmediate(nodeObj.GetComponent(removedScript.name));
            //     }
            // }
            // else
            // {
            //     // Detect changes in the list (same count, different elements or order)
            //     for (int i = 0; i < previousScripts.Count; i++)
            //     {
            //         if (previousScripts[i] != node.attachedScripts[i])
            //         {
            //             Debug.Log($"Script Changed: Previous: {previousScripts[i]?.name} => Current: {node.attachedScripts[i]?.name}");
            //             if(previousScripts[i] is not null && previousScripts[i] != previousScripts[i-1]){
            //                 Type componentType = Type.GetType(previousScripts[i].name);
            //                 DestroyImmediate(nodeObj.GetComponent(componentType));
            //             }

            //             if(node.attachedScripts[i] is not null){
            //                 nodeObj.AddComponent(node.attachedScripts[i].GetType());
            //             }
                    
            //         }
            //     }
            // }
            
            //Debug.Log("attachedScripts list has been changed!");

            // For example, you can update the GameObject corresponding to the node
            // node.UpdateGameObject();  // Assuming you have a method like this in your PAT_Node class
            
            // If you need to do something more, like updating visuals in your custom Node Editor Window, you can add it here.
        }

        

    }
#endif  
}
#endif