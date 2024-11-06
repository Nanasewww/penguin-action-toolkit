using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
namespace PAT
{
    [ExecuteAlways]
    
    public class PAT_GraphRunner : MonoBehaviour
    {
        // Start is called before the first frame update
        public PAT_NodeGraph nodeGraph;
        
        public void Initialize(PAT_NodeGraph nodeGraph){
            nodeGraph.isInitialzed = true;
            nodeGraph.rootNode.objid = gameObject.GetInstanceID();
            int index = nodeGraph.nodes.IndexOf(nodeGraph.rootNode);
            nodeGraph.nodeObjIds[index] = nodeGraph.rootNode.objid;
            //nodeGraph.nodeDict[nodeGraph.rootNode] = nodeGraph.rootNode.objid;
            nodeGraph.rootNode.isInitialized = true;
            if (gameObject.name != nodeGraph.rootNode.title){
                gameObject.name= nodeGraph.rootNode.title;
            }
            foreach(MonoScript characterScript in nodeGraph.rootNode.attachedScripts ){
                if(characterScript is not null){
                    System.Type characterScriptClass = characterScript.GetClass();
                    gameObject.AddComponent(characterScriptClass);
                    }
                }

            foreach(PAT_MoveSetNode moveset in nodeGraph.rootNode.children){
                GameObject MoveSetObj = new GameObject(moveset.title);
                UnityEngine.Debug.Log("createObj" + MoveSetObj.name);
                moveset.objid = MoveSetObj.GetInstanceID();
                index = nodeGraph.nodes.IndexOf(moveset);
                nodeGraph.nodeObjIds[index] = moveset.objid;
                //nodeGraph.nodeDict[moveset] = moveset.objid;
                moveset.isInitialized=true;
                MoveSetObj.transform.parent = this.gameObject.transform;

                if(moveset.attachedScripts is not null){
                    foreach(MonoScript movesetScript in moveset.attachedScripts ){
                        System.Type movesetScriptClass = movesetScript.GetClass();
                        MoveSetObj.AddComponent(movesetScriptClass);
                    }
                }
                
                
                foreach(PAT_ActionNode action in moveset.children){
                    GameObject ActionObj = new GameObject(action.title);
                    UnityEngine.Debug.Log("createObj" + ActionObj.name);
                    action.objid = ActionObj.GetInstanceID();
                    //nodeGraph.nodeDict[action] = action.objid;
                    index = nodeGraph.nodes.IndexOf(action);
                    nodeGraph.nodeObjIds[index] = action.objid;
                    action.isInitialized = true;
                    ActionObj.transform.parent = MoveSetObj.transform;

                    if(action.attachedScripts is not null){
                        foreach(MonoScript actionScript in action.attachedScripts ){
                            System.Type actionScriptClass = actionScript.GetClass();
                            ActionObj.AddComponent(actionScriptClass);
                        }
                    }
                    foreach(PAT_ModifierNode modifier in action.children){
                        GameObject ModifierObj = new GameObject(modifier.title);
                        UnityEngine.Debug.Log("createObj" + ModifierObj.name);
                        modifier.objid = ModifierObj.GetInstanceID();
                        //nodeGraph.nodeDict[modifier] = modifier.objid;
                        index = nodeGraph.nodes.IndexOf(modifier);
                        nodeGraph.nodeObjIds[index] = modifier.objid;
                        modifier.isInitialized = true;
                        ModifierObj.transform.parent = ActionObj.transform;
                        
                        if(modifier.attachedScripts is not null){
                            foreach(MonoScript modifierScript in modifier.attachedScripts ){
                                System.Type modifierScriptClass = modifierScript.GetClass();
                                ModifierObj.AddComponent(modifierScriptClass);
                            }
                        }
                    }
                }
            }
        }

        // public void UpdateGraph(PAT_NodeGraph nodeGraph){
            
        // }

        // public void UpdateCharacter(PAT_NodeGraph nodeGraph){
            
            
        // }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(PAT_GraphRunner))]
    public class PAT_GraphRunnerInspector : Editor
    {
        // Start is called before the first frame update
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            PAT_GraphRunner runner = (PAT_GraphRunner)target;
            if(GUILayout.Button("Initialzie PAT Character")){
                runner.Initialize(runner.nodeGraph);
                // GameObject modifier = GameObject.Find("Modifier");
                // Selection.objects = new Object[] { modifier };
            }

            if(GUILayout.Button("Update PAT Graph")){
                // runner.UpdateGraph(runner.nodeGraph);
                // GameObject modifier = GameObject.Find("Modifier");
                // Selection.objects = new Object[] { modifier };
            }

            if(GUILayout.Button("Update PAT Character")){
                // runner.UpdateCharacter(runner.nodeGraph);
                // GameObject modifier = GameObject.Find("Modifier");
                // Selection.objects = new Object[] { modifier };
            }
        }
    }   
#endif
}

#endif