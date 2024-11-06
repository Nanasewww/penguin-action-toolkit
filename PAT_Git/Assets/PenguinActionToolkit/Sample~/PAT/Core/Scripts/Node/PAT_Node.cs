using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
namespace PAT
{
    
    public abstract class PAT_Node : ScriptableObject
    {

        [HideInInspector]
        public string guid;
        [HideInInspector]
        public int objid;
        [HideInInspector]
        public bool isInitialized = false;
        [HideInInspector]
        public bool parentIsChanged = false;
        //public bool markForDelete = false;
        [HideInInspector]
        public Vector2 position;

        [HideInInspector]
        public string title;
        
        public List<PAT_Node> children = new List<PAT_Node>();
        public List<MonoScript> attachedScripts;
        
        // private void OnTextFieldChanged(ChangeEvent<string> evt)
        // {
        //     // Handle the value change and update the graph as needed
        //     Debug.Log($"TextField value changed to: {evt.newValue}");
        //     PAT_Manager_NodeWindow graphView = GetFirstAncestorOfType<>
        //     // Trigger a refresh on the graph
        //     if (graphView != null)
        //     {
        //         graphView.ScheduleUpdate(); // Schedule a refresh
        //     }
        // }
        
    }

    
}
#endif