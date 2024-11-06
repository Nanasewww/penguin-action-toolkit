using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;


namespace PAT
{
    public class PAT_Manager_InspectorWindow : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<PAT_Manager_InspectorWindow, VisualElement.UxmlTraits>{}
        Editor editor;
        public PAT_Manager_InspectorWindow(){
            
        }

        internal void UpdateSelection(PAT_Manager_NodeVisual nodeVisual){
            Clear();
            UnityEngine.Object.DestroyImmediate(editor);
            editor = Editor.CreateEditor(nodeVisual.node);
            IMGUIContainer container = new IMGUIContainer(()=>{editor.OnInspectorGUI();});
            Add(container);
        }
    }
}
