using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PAT
{
    [Serializable]
    public struct DataKey
    {
        public string ID;
        public bool status;
    }
    
    [Serializable]
    public class PlayerData
    {
        public int maxHp = 200;
        public int atk = 150;
        public List<DataKey> keys = new();
        public string currentScene = "";
        public int spawnPointId = 0;
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "SavaData", order = 1)]
    public class SaveData: ScriptableObject
    {
        protected string saveFilePath;

        public PlayerData data;

        private void OnEnable()
        {
            saveFilePath =  Application.persistentDataPath +"/PlayerData.json";
        }

        [ContextMenu("Save")]
        public void SaveGame()
        {
            string content = JsonUtility.ToJson(data);
            File.WriteAllText( saveFilePath, content);
            Debug.Log("Saved to :" + saveFilePath);
        }
        
        [ContextMenu("Load")]
        public void LoadGame()
        {
            if (File.Exists(saveFilePath))
            {
                string loadPlayerData = File.ReadAllText(saveFilePath);
                data = JsonUtility.FromJson<PlayerData>(loadPlayerData);
                Debug.Log("Loaded");
            }
            else
            {
                Debug.Log("No save Data Found");
            }
        }
  
        [ContextMenu("Delete")]
        public void DeleteSaveFile()
        {
            if (File.Exists(saveFilePath))
            {
                File.Delete( saveFilePath);
                Debug.Log("Deleted");
            }
        }
    }
    

}