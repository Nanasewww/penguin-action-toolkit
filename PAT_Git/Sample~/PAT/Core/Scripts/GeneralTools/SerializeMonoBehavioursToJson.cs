#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
namespace PAT
{
    public class SerializeMonoBehavioursToJson
    {
        [MenuItem("Tools/Serialize All MonoBehaviours in Folder to JSON")]
        public static void SerializeAllMonoBehavioursInFolder()
        {
            // Set the search to include all folders by using "Assets" as the root folder
            string[] assetGuids = AssetDatabase.FindAssets("t:GameObject", new[] { "Assets" });

            // List to hold JSON outputs
            List<string> jsonOutputs = new List<string>();

            foreach (string guid in assetGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (obj != null)
                {
                    // Loop through all MonoBehaviours attached to the GameObject
                    AttackWithHitboxMod[] monoBehaviours = obj.GetComponentsInChildren<AttackWithHitboxMod>();
                    foreach (AttackWithHitboxMod mb in monoBehaviours)
                    {
                        if (mb != null)
                        {
                            // Convert the MonoBehaviour to JSON
                            string json = JsonUtility.ToJson(mb, true);
                            jsonOutputs.Add($"Object: {obj.name}, Component: {mb.GetType().Name}\n{json}");
                        }
                    }
                }
            }

            // Save the JSON data to a file
            string outputPath = Path.Combine(Application.dataPath, "SerializedData.json");
            File.WriteAllLines(outputPath, jsonOutputs.ToArray());

            Debug.Log($"Serialized data written to {outputPath}");
            AssetDatabase.Refresh(); // Refresh the Asset Database if you want to view the file in the Editor
        }
    }
}
#endif