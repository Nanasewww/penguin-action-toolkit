using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PAT
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public SaveData saveDataContainer;
        public Character player;
        public GameObject playerPrefab;
        public List<SpawnPointUnit> spawnPoints = new();

        public GameManager(Character player)
        {
            this.player = player;
        }

        public event Action GameOver;
        
        private void Awake()
        {
            Instance = this;

            if (!saveDataContainer) saveDataContainer = (SaveData)Resources.Load("Data");
            if (saveDataContainer) saveDataContainer.LoadGame();
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Start()
        {
            if(!player) return;
            
            List<SpawnPointUnit> scenePoints = FindObjectsOfType<SpawnPointUnit>().ToList();

            foreach (SpawnPointUnit point in scenePoints)
            {
                if(spawnPoints.Contains(point)) continue;
                spawnPoints.Add(point);
            }
            
            SpawnPointUnit p = GetSpawnPointByID(saveDataContainer.data.spawnPointId);
            player.transform.position = p.spawnTransform.position;
            player.transform.rotation = p.spawnTransform.rotation;

            player.healthAttribute.OnBaseValueReachMin += () => { StartCoroutine(GameEndSequence()); };
        }
        
        IEnumerator GameEndSequence()
        {
            GameOver?.Invoke();
            yield return new WaitForSecondsRealtime(1.5f);
            //LoadScene("EndDeath");
            LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneSequence(sceneName));
        }
        
        IEnumerator LoadSceneSequence(string sceneName)
        {
            yield return new WaitForSecondsRealtime(1.5f);
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }
        
        public SpawnPointUnit GetSpawnPointByID(int id)
        {
            if (spawnPoints.Count == 0) return null;

            foreach (SpawnPointUnit spawnPoint in spawnPoints)
            {
                if (spawnPoint.id == id) return spawnPoint;
            }

            return null;
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            //Todo: make this work again with player
            //InputManager.Input.UIMode();
        }

        public void UnPauseGame()
        {
            Time.timeScale = 1;
            //Todo: make this work again with player
            //InputManager.Input.GameMode();
        }

        private void OnDestroy()
        {
            UnPauseGame();
        }
    }
}
