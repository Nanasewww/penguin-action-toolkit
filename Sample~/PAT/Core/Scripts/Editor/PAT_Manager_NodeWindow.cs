using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using Unity.Plastic.Antlr3.Runtime.Tree;
using System.Diagnostics;
using System.Linq;
using UnityEngine.InputSystem;

namespace PAT
{
    public class PAT_Manager_NodeWindow : GraphView{

        public new class UxmlFactory : UxmlFactory<PAT_Manager_NodeWindow, GraphView.UxmlTraits>{}
        PAT_NodeGraph tree;
        public Action<PAT_Manager_NodeVisual> OnNodeSelected;
        public PAT_Manager_NodeWindow(){
            Insert(0 ,new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/PenguinActionToolkit/Core/Scripts/Editor/PAT_Manager_EditorStyle.uss");
            styleSheets.Add(styleSheet);
        }
        
        PAT_Manager_NodeVisual FindNodeVisual(PAT_Node node){
            return GetNodeByGuid(node.guid) as PAT_Manager_NodeVisual;
        }
        internal void PopulateView(PAT_NodeGraph tree){
            this.tree = tree;
            
            graphViewChanged -= OnGraphViewChanged;
            //bool isRoot = graphElements is PAT_CharacterNode;
            DeleteElements(graphElements);
            // UnityEngine.Debug.Log(graphElements.GetType());
            graphViewChanged += OnGraphViewChanged;
            
            if(tree.rootNode == null){
                tree.rootNode = tree.CreateNode(typeof(PAT_CharacterNode)) as PAT_CharacterNode;
                tree.rootNode.Initialize();
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }
            
            //create node visual
            // foreach(PAT_Node n in tree.nodeDict.Keys){
            //     CreateNodeView(n);
            // }
            //tree.nodeDict.GetKeys().ForEach(n => CreateNodeView(n));
            tree.nodes.ForEach(n => CreateNodeView(n));
            //create edge visual
            tree.nodes.ForEach( n => {
                var children = tree.GetChildren(n);
                children.ForEach( c=>{
                    PAT_Manager_NodeVisual parentNodeVisual = FindNodeVisual(n);
                    PAT_Manager_NodeVisual childNodeVisual = FindNodeVisual(c);
                    
                    Edge edge = parentNodeVisual.output.ConnectTo(childNodeVisual.input);
                    AddElement(edge);
                });
            });
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            PAT_Manager_NodeVisual startVisualNode = startPort.node as PAT_Manager_NodeVisual;
            List<Port> returnPort = new List<Port>();
            List<Port> currentPort = ports.ToList();

            foreach (Port endPort in currentPort){
                if (endPort.direction != startPort.direction && endPort.node != startPort.node){
                    PAT_Manager_NodeVisual endVisualNode = endPort.node as PAT_Manager_NodeVisual;
                    if (startVisualNode.node is PAT_CharacterNode && endVisualNode.node is PAT_MoveSetNode){
                        returnPort.Add(endPort);
                    }else if(startVisualNode.node is PAT_MoveSetNode && endVisualNode.node is PAT_ActionNode){
                        returnPort.Add(endPort);
                    }else if(startVisualNode.node is PAT_ActionNode && endVisualNode.node is PAT_ModifierNode){
                        returnPort.Add(endPort);
                    }
                }
            }
            return returnPort;
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange){
            if (graphViewChange.elementsToRemove != null){

                // List<PAT_Manager_NodeVisual> nodesToRemove = new List<PAT_Manager_NodeVisual>();
                // List<Edge> edgesToRemove = new List<Edge>();
                // foreach (var elem in graphViewChange.elementsToRemove)
                // {
                //     PAT_Manager_NodeVisual nodeView = elem as PAT_Manager_NodeVisual;
                //     if (nodeView != null)
                //     {
                //         nodesToRemove.Add(nodeView);
                //     }

                //     Edge edge = elem as Edge;
                //     if (edge != null)
                //     {
                //         edgesToRemove.Add(edge);
                //     }
                // }

                // foreach (var nodeVisual in nodesToRemove)
                // {
                //     if (nodeVisual.node is PAT_CharacterNode)
                //     {
                //         // Skip character nodes if necessary
                //         graphViewChange.elementsToRemove.Remove(nodeVisual);
                //     }
                //     else
                //     {
                //         // Process node deletion and log the children count
                //         UnityEngine.Debug.Log($"Children count before deletion: {nodeVisual.node.children.Count}");
                //         tree.DeleteNode(nodeVisual.node);
                //         UnityEngine.Debug.Log($"Node {nodeVisual.node.name} deleted, children count: {nodeVisual.node.children.Count}");
                //     }
                // }

                // foreach (var edge in edgesToRemove)
                // {
                //     PAT_Manager_NodeVisual parentNode = edge.output.node as PAT_Manager_NodeVisual;
                //     PAT_Manager_NodeVisual childNode = edge.input.node as PAT_Manager_NodeVisual;
                //     tree.RemoveChild(parentNode.node, childNode.node);
                // }

                
                graphViewChange.elementsToRemove.ForEach(elem => {
                    
                    PAT_Manager_NodeVisual nodeView = elem as PAT_Manager_NodeVisual;
                
                    if (nodeView != null){
                        if(nodeView.node is PAT_CharacterNode){
                            graphViewChange.elementsToRemove.Remove(elem);
                        }else{
                            tree.DeleteNode(nodeView.node);
                            //UnityEngine.Debug.Log("Node Delete!");
                        }
                    }

                    Edge edge = elem as Edge;
                    if (edge != null){
                        PAT_Manager_NodeVisual parentNode = edge.output.node as PAT_Manager_NodeVisual;
                        PAT_Manager_NodeVisual childNode = edge.input.node as PAT_Manager_NodeVisual;
                        tree.RemoveChild(parentNode.node, childNode.node);
                        //UnityEngine.Debug.Log("Edge Delete!");
                    }
                    
                });
            }

            if(graphViewChange.edgesToCreate != null){
                graphViewChange.edgesToCreate.ForEach(edge =>{
                    PAT_Manager_NodeVisual parentNode = edge.output.node as PAT_Manager_NodeVisual;
                    PAT_Manager_NodeVisual childNode = edge.input.node as PAT_Manager_NodeVisual;
                    if( (parentNode.node is PAT_CharacterNode && childNode.node is PAT_MoveSetNode)||
                        (parentNode.node is PAT_MoveSetNode && childNode.node is PAT_ActionNode)||
                        (parentNode.node is PAT_ActionNode && childNode.node is PAT_ModifierNode)
                        ){
                            tree.AddChild(parentNode.node, childNode.node);
                        }else{
                            Remove(edge);
                        }
                });
            }
            return graphViewChange;
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            var types = TypeCache.GetTypesDerivedFrom<PAT_Node>();
            foreach(var type in types){
                if( type.Name != "PAT_CharacterNode"){
                    Vector2 mousePos = evt.mousePosition;
                    evt.menu.AppendAction($"Create {type.Name}",(a) => CreateNode(type,mousePos));
                }
            }
            
        }

        internal void CreateNode(System.Type type, Vector2 pos){
            PAT_Node node = tree.CreateNode(type);
            //Vector2 graphMousePosition = this.ChangeCoordinatesTo(this.contentViewContainer, pos);
            Vector2 graphMousePosition = this.contentViewContainer.WorldToLocal(pos);
            node.position = graphMousePosition;
            //UnityEngine.Debug.Log(node.position);
            CreateNodeView(node);
        }
        internal void CreateNodeView(PAT_Node node){
            PAT_Manager_NodeVisual nodeVisual = new PAT_Manager_NodeVisual(node);
            nodeVisual.OnNodeSelected = OnNodeSelected;
            // if (Event.current != null){
            //     Vector2 mousePosition = Event.current.mousePosition;
            //     Vector2 graphMousePosition = this.ChangeCoordinatesTo(this.contentViewContainer, mousePosition);
            //     node.position = graphMousePosition;
            // }
            AddElement(nodeVisual);
            //nodeVisual.SetPosition(new Rect(this.contentViewContainer.WorldToLocal( Mouse.current.position.ReadValue() ),new Vector2 (100,100)));
        }

    }

    
}
