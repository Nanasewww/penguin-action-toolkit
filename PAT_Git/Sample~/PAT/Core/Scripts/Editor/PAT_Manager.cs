using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;

namespace PAT{
    public class PAT_Manager : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;
        PAT_Manager_NodeWindow nodeWindow;
        PAT_Manager_InspectorWindow inspectorWindow;
        
        [MenuItem("PAT_Toolkit/PAT_Manager")]
        public static void OpenWindow()
        {
            PAT_Manager wnd = GetWindow<PAT_Manager>();
            wnd.titleContent = new GUIContent("PAT_Manager");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line){
            if(Selection.activeObject is PAT_NodeGraph){
                OpenWindow();
                return true;
            }
            return false;
        }
        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Instantiate UXML
            // VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
            // root.Add(labelFromUXML);
            m_VisualTreeAsset.CloneTree(root);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/PenguinActionToolkit/Core/Scripts/Editor/PAT_Manager_EditorStyle.uss");
            root.styleSheets.Add(styleSheet);

            nodeWindow = root.Q<PAT_Manager_NodeWindow>();
            inspectorWindow = root.Q<PAT_Manager_InspectorWindow>();
            nodeWindow.OnNodeSelected = OnNodeSelectionChagned;

            OnSelectionChange();
        }

        private void OnSelectionChange(){
            PAT_NodeGraph tree = Selection.activeObject as PAT_NodeGraph;
            if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())){
                nodeWindow.PopulateView(tree);
            }
        }

        void OnNodeSelectionChagned(PAT_Manager_NodeVisual node){
            inspectorWindow.UpdateSelection(node);
        }
    }
}