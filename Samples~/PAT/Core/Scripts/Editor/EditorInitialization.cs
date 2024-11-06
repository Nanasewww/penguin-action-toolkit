using UnityEditor;
using UnityEngine;

namespace PAT
{
    [InitializeOnLoad]
    public static class EditorInitialization
    {
        static EditorInitialization()
        {
            // This runs when the editor loads
            EditorApplication.delayCall += InitializeScriptableObjects;
        }

        private static void InitializeScriptableObjects()
        {
            string[] guids = AssetDatabase.FindAssets("t:GamePlayTagHelper"); // Replace with your ScriptableObject class name

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GamePlayTagHelper instance = AssetDatabase.LoadAssetAtPath<GamePlayTagHelper>(path);
                instance.Awake();
            }
        }
    }
}
