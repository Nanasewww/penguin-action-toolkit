using System.Collections;
using System.Collections.Generic;
// using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using System.Data;
#if UNITY_EDITOR
namespace PAT
{
    [CreateAssetMenu()]
    //One PAT Node graph is only for one character structure, which means only one character node can be contained in the graph
    public class PAT_NodeGraph : ScriptableObject
    {
        public PAT_CharacterNode rootNode;
        public bool isInitialzed = false;
        public int characterObj;
        
        //public PAT_SerializableDictionary<PAT_Node,int> nodeDict = new PAT_SerializableDictionary<PAT_Node, int>();
        public List<PAT_Node> nodes = new List<PAT_Node>();
        public List<int> nodeObjIds = new List<int>();
        public PAT_Node CreateNode(System.Type type){
            PAT_Node node = ScriptableObject.CreateInstance(type) as PAT_Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            //nodeDict.Add(node,0);
            nodes.Add(node);
            nodeObjIds.Add(0);
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
            if( node is PAT_MoveSetNode){
                PAT_MoveSetNode moveset = node as PAT_MoveSetNode;
                moveset.Initialize();
            }
            return node;
        }

        public void DeleteNode(PAT_Node node){
            bool patChildExisted = false;
            // foreach (KeyValuePair<PAT_Node, int> entry in nodeDict)
            // {
            //     Debug.Log("Key: " + entry.Key.title + ", Value: " + entry.Value);
            // }
            if( isInitialzed && node.isInitialized ){
                node.parentIsChanged = true;
                GameObject nodeObj =  (GameObject)EditorUtility.InstanceIDToObject(node.objid);
                
                if(nodeObj.transform.childCount > 0){
                    List<GameObject> children = new List<GameObject>();
                    foreach (Transform child in nodeObj.transform)
                    {
                        children.Add(child.gameObject);
                        int childID = child.gameObject.GetInstanceID();
                        if( nodeObjIds.Contains(childID)){
                            patChildExisted = true;
                            Debug.Log("pat child existed! Inactive the children");
                        }
                        // Access the child GameObject using child.gameObject
                    }
                    Debug.Log(patChildExisted);
                    if(patChildExisted){
                        foreach (GameObject child in children)
                        {
                            child.transform.parent = nodeObj.transform.parent;
                            Debug.Log(nodeObj.transform.parent);
                            child.SetActive(false);
                            Debug.Log("Test");
                        }
                    }
                
                }

                DeleteNodeObj(node);
                
            }
            int index = nodes.IndexOf(node);
            nodeObjIds.RemoveAt(index);
            nodes.Remove(node);
            //nodeDict.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }
        
        public void AddChild(PAT_Node parent, PAT_Node child){
            
            bool passed = false;

            if( parent is PAT_CharacterNode && child is PAT_MoveSetNode ){
                PAT_CharacterNode character = parent as PAT_CharacterNode;
                PAT_MoveSetNode moveset = child as PAT_MoveSetNode;
                character.children.Add(moveset);
                passed = true;
            }else if(parent is PAT_MoveSetNode && child is PAT_ActionNode){
                PAT_MoveSetNode moveset = parent as PAT_MoveSetNode;
                PAT_ActionNode action = child as PAT_ActionNode;
                moveset.children.Add(action);
                passed = true;
            }else if(parent is PAT_ActionNode && child is PAT_ModifierNode){
                PAT_ActionNode action = parent as PAT_ActionNode;
                PAT_ModifierNode modifier = child as PAT_ModifierNode;
                action.children.Add(modifier);
                passed = true;
            }

            if(passed){child.parentIsChanged = true;}

            if(isInitialzed && passed){
                AddNodeObj(parent, child);
            }

        }

        public void RemoveChild(PAT_Node parent, PAT_Node child){

            bool passed = false;
            
            PAT_CharacterNode character = parent as PAT_CharacterNode;
            PAT_MoveSetNode moveset = child as PAT_MoveSetNode;
            if (character){
                character.children.Remove(moveset);
                passed = true;
            }

            moveset = parent as PAT_MoveSetNode;
            PAT_ActionNode action = child as PAT_ActionNode;
            if (moveset){
                moveset.children.Remove(action);
                passed = true;
            }

            action = parent as PAT_ActionNode;
            PAT_ModifierNode modifier = child as PAT_ModifierNode;
            if (action){
                action.children.Remove(modifier);
                passed = true;
            }

            if (isInitialzed & passed){
                child.parentIsChanged = true;
                InactivateNodeObj(child);
            }
        }

        
        public List<PAT_Node> GetChildren(PAT_Node parent){
            List<PAT_Node> children = new List<PAT_Node>();

            if( parent is PAT_CharacterNode){
                PAT_CharacterNode character = parent as PAT_CharacterNode;
                return character.children;
            }else if( parent is PAT_MoveSetNode){
                PAT_MoveSetNode moveset = parent as PAT_MoveSetNode;
                return moveset.children;
            }else if( parent is PAT_ActionNode){
                PAT_ActionNode action = parent as PAT_ActionNode;
                return action.children;
            }
            

            return children;
        }

        public void AddNodeObj(PAT_Node parent, PAT_Node child){
            if(parent.isInitialized == false || EditorUtility.InstanceIDToObject(parent.objid) is null ){
                UnityEngine.Debug.LogError("Should not call add node obj if parent is not initialized");
            }else{
                GameObject childObj;
                GameObject parentObj = (GameObject)EditorUtility.InstanceIDToObject(parent.objid);
                if(child.parentIsChanged){

                    if(child.isInitialized){
                        childObj =  (GameObject)EditorUtility.InstanceIDToObject(child.objid);
                        childObj.transform.parent = parentObj.transform;
                        child.parentIsChanged = false;
                        childObj.SetActive(true);
                    }else{
                        childObj = CreateNodeObj(child);
                        childObj.transform.parent = parentObj.transform;
                        child.parentIsChanged = false;
                    }

                    foreach(PAT_Node child_child in child.children ){
                        AddNodeObj(child, child_child);
                    }
                }
            }
        }

        public void InactivateNodeObj(PAT_Node node){
            if(node.isInitialized == false || EditorUtility.InstanceIDToObject(node.objid) is null){
                UnityEngine.Debug.LogError("Should not Inactivate node obj if node is not initialized");
            }else{
                GameObject nodeObj = (GameObject)EditorUtility.InstanceIDToObject(node.objid);
                nodeObj.SetActive(false);
            }
        }


        public GameObject CreateNodeObj(PAT_Node node){
            if(node.isInitialized == false && EditorUtility.InstanceIDToObject(node.objid) is null ){
                GameObject NodeObj = new GameObject(node.title);
                node.objid = NodeObj.GetInstanceID();
                int index = nodes.IndexOf(node);
                nodeObjIds[index] = node.objid;
                //nodeDict[node] = node.objid;
                node.isInitialized = true;
                if(node.attachedScripts is not null){
                    foreach(MonoScript script in node.attachedScripts ){
                        System.Type scriptClass = script.GetClass();
                        NodeObj.AddComponent(scriptClass);
                    }
                }
                return NodeObj;
            }else{
                UnityEngine.Debug.LogError("Should not initialize node if it is already initialized");
                return null;
            }
        }

        public void DeleteNodeObj(PAT_Node node){
            if(node.isInitialized == false || EditorUtility.InstanceIDToObject(node.objid) is null ){
                UnityEngine.Debug.LogError("Should not delete node obj if node is not initialized");
            }else{
                GameObject nodeObj = (GameObject)EditorUtility.InstanceIDToObject(node.objid);
                foreach(PAT_Node child in node.children){
                    InactivateNodeObj(child);
                    child.parentIsChanged = true;
                }
                DestroyImmediate(nodeObj);
            }
        }
    }
}
#endif