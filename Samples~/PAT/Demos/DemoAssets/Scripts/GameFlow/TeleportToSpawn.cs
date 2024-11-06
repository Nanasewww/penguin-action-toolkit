using UnityEngine;
using UnityEngine.SceneManagement;

namespace PAT
{
    public class TeleportToSpawn: MonoBehaviour
    {
        public string sceneName;
        public int id;

        void Start()
        {
            sceneName = SceneManager.GetActiveScene().name;
        }
        
        public void Teleport()
        {
            SaveData dataFile = GameManager.Instance.saveDataContainer;
            dataFile.data.spawnPointId = id;
            dataFile.SaveGame();
            
            GameManager.Instance.LoadScene(sceneName);
        }
    }
    

}