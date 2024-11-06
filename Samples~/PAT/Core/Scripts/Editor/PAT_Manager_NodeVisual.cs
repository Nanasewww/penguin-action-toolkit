using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using System.Linq;
using Unity.VisualScripting;

namespace PAT
{
    public class PAT_Manager_NodeVisual : UnityEditor.Experimental.GraphView.Node
    {
        public Action<PAT_Manager_NodeVisual> OnNodeSelected;
        public PAT_Node node;
        public Port input;
        public Port output;
        private VisualElement scriptContainer;
        public PAT_Manager_NodeVisual(PAT_Node node): base("Assets/PenguinActionToolkit/Core/Scripts/Editor/PAT_NodeVisual.uxml"){
            this.node = node;
            scriptContainer = this.Q<VisualElement>("scripts");
            

            if(node.title == null){
                node.title = node.name;
                //Debug.Log("Initialzie!");
            }
            
            this.title = node.title;
            
            // if(node.title.Length == 0){
            //     this.title = node.name;
            // }else{
            //     this.title = node.title;
            // }
            this.viewDataKey = node.guid;
    
            style.left = node.position.x;
            style.top = node.position.y;
            
            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();

            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            RegisterCallback<DragPerformEvent>(OnDragPerform);
            // this.AddManipulator(new Clickable(() =>
            // {
            //     // Select the GameObject with the same name
            //     SelectCorrespondingGameObject();
            // }) );
        }

        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            // Check if the dragged object is a MonoScript
            if (DragAndDrop.objectReferences.Any(obj => obj is MonoScript))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
        }

        private void OnDragPerform(DragPerformEvent evt)
        {
            // Get the object that was dropped
            foreach (var obj in DragAndDrop.objectReferences)
            {
                MonoScript script = obj as MonoScript;

                // Check if it's a valid MonoScript and if the script derives from MonoBehaviour
                if (script != null && typeof(MonoBehaviour).IsAssignableFrom(script.GetClass()))
                {
                    // Add the script to the attachedScripts list in the node
                    node.attachedScripts.Add(script);
                    if(node.isInitialized){
                        GameObject nodeObj =  (GameObject)EditorUtility.InstanceIDToObject(node.objid);
                        nodeObj.AddComponent(script.GetClass());
                    }

                    // Refresh the node visual (you might want to update UI elements to show the added script)
                    // RefreshNodeVisual();
                }
            }
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Edit Title", EditTitle);
            evt.menu.AppendAction("Select corresponded GameObject", SelectCorrespondingGameObject);
        }
        
        private void SelectCorrespondingGameObject(DropdownMenuAction action)
        {
            // Find the GameObject with the same name
            GameObject correspondingObject = (GameObject)EditorUtility.InstanceIDToObject(this.node.objid);
            //GameObject.Find(this.node.title);
            
            if (correspondingObject != null)
            {
                // Select the GameObject in the Inspector
                Selection.activeGameObject = correspondingObject;
            }
            else
            {
                Debug.LogWarning($"No GameObject found with the name: {this.node.title}");
            }
        }

        private void EditTitle(DropdownMenuAction action)
        {
            var editorWindow = EditorWindow.focusedWindow;
            UnityEditor.PopupWindow.Show(new Rect(action.eventInfo.mousePosition, Vector2.zero), new PAT_Manager_TitlePopup(title, (newTitle) =>
            {
                // Update the node title when the user clicks 'Apply'
                node.title = newTitle;
                this.title = node.title;
                if(node.isInitialized){
                    GameObject nodeObj = (GameObject)EditorUtility.InstanceIDToObject(node.objid);
                    nodeObj.name = node.title;
                }

            }));
        }

        

        private void SetupClasses(){
            if (node is PAT_CharacterNode){
                AddToClassList("character");
            }else if(node is PAT_MoveSetNode){
                AddToClassList("moveset");
            }else if(node is PAT_ActionNode){
                AddToClassList("action");
            }else if(node is PAT_ModifierNode){
                AddToClassList("modifier");
            }
        }
        private void CreateInputPorts(){
            if (node is PAT_CharacterNode){
                
            }else{  
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            //input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            if (input != null){
                input.portName = "";
                input.style.flexDirection = FlexDirection.Column;
                VisualElement connectorBox = input.Children().ToList()[0];
                connectorBox.style.height = 12;
                connectorBox.style.width = 12;
                inputContainer.Add(input);
            }
        }

        private void CreateOutputPorts(){
            if (node is PAT_ModifierNode){

            }else{
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }

            
            if (output != null){
                output.portName = "";
                output.style.flexDirection = FlexDirection.ColumnReverse;
                VisualElement connectorBox = output.Children().ToList()[0];
                connectorBox.style.height = 12;
                connectorBox.style.width = 12;
                outputContainer.Add(output);
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            if(OnNodeSelected != null){
                OnNodeSelected.Invoke(this);
            }
        }
        

    }
}
