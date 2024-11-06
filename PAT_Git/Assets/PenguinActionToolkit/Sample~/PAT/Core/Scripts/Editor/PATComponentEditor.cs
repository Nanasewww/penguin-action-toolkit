using UnityEditor;
using UnityEngine;


namespace PAT
{

    [CustomEditor(typeof(PATComponent), true)]
    [CanEditMultipleObjects]
    public class PATComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PATComponent holder = (PATComponent)target;

            if (holder.attributes == null)
            {
                DrawDefaultInspector();
                return;
            }
            // Display the list of attributes
            for (int i = 0; i < holder.attributes.Count; i++)
            {
                Attribute attribute = holder.attributes[i];
                if (attribute != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(attribute.resourceTag.ToString(), GUILayout.MaxWidth(100));
                    EditorGUILayout.LabelField("Current: " + attribute.currentAmount, GUILayout.MaxWidth(150));
                    EditorGUILayout.LabelField(" Base: ",GUILayout.MaxWidth(50));
                    attribute.SetBaseValue(EditorGUILayout.FloatField(attribute.GetBaseValue(), GUILayout.ExpandWidth(true)));
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.LabelField("Attribute " + (i + 1), "Missing Reference");    
                }
            }

            // Show the default inspector for the attributes list
            DrawDefaultInspector();
        }
    }
}
