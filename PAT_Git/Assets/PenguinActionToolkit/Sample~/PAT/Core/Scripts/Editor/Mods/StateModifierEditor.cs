using UnityEditor;

namespace PAT
{
    [CustomEditor(typeof(StateModifier), true)]
    [CanEditMultipleObjects]
    public class StateModifierEditor : Editor
    {
        private bool showSection1 = false;
        SerializedProperty mode;


        private void OnEnable()
        {
            mode = serializedObject.FindProperty("mode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Show certain fields by default
            /*EditorGUILayout.PropertyField(serializedObject.FindProperty("field1"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("field2"));*/

            // Create a foldout section for other fields
            EditorGUILayout.PropertyField(serializedObject.FindProperty("modName"));
            showSection1 = EditorGUILayout.Foldout(showSection1, "Timing Attribute");
            if (showSection1)
            {
                EditorGUILayout.PropertyField(mode);
                switch ((ModifierMode)mode.enumValueIndex)
                {
                    case ModifierMode.ByAnimationEvent:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_beginIndex"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_endIndex"));
                        break;
                    case ModifierMode.ByTimeInState:
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_beginTime"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("_endTime"));
                        break;
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_events"));
            }
            
            serializedObject.ApplyModifiedProperties();

            // Another foldout section
            /*showSection2 = EditorGUILayout.Foldout(showSection2, "Section 2");
            if (showSection2)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("field5"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("field6"));
            }*/

            
            DrawPropertiesExcluding(serializedObject, "mode", "_beginIndex", "_endIndex", "_beginTime", "_endTime", "_events", "m_Script", "modName");
            serializedObject.ApplyModifiedProperties();
        }
    }
}
