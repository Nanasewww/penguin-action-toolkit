using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace PAT
{
    public class LevelManager: MonoBehaviour
    {
        [Header("GameProgress")]
        public int rescuedRabit;

        [Header("Enemies")] 
        public GameObject minionPrefab;
        public GameObject elitePrefab;
        public Character eliteInstance;

        [Header("Timer")] 
        public float remainingTime = 180f;
        public UnityEvent secondWave;

        [Header("UI")] 
        public TextMeshProUGUI follower;
        public TextMeshProUGUI timer;

        protected bool doom = false;
        protected bool second = false;
        protected bool ended = false;
        private void Update()
        {
            remainingTime -= Time.deltaTime;
            
            if(!second && remainingTime <= 90){second = true; secondWave.Invoke();}
            if(remainingTime <=0) {doom = true; EndGame(); return;}

            //Update rescued Rabit
            rescuedRabit = 0;
            foreach (AiBrain ai in AiBrain.brains)
            {
                if (ai.Leader == Player.Players[0].character && !ai.target) rescuedRabit++;
            }
            
            //Update UI
            follower.text = rescuedRabit.ToString();
            timer.text = Mathf.RoundToInt(remainingTime).ToString();
        }

        public void EndGame()
        {
            if(ended) return;

            ended = true; 
            
            if (doom)
            {
                GameManager.Instance.LoadScene("EndDoom"); 
            }
            else switch (rescuedRabit)
            {
                case > 9:
                    GameManager.Instance.LoadScene("End3");
                    break;
                case > 3:
                    GameManager.Instance.LoadScene("End2");
                    break;
                case > 0:
                    GameManager.Instance.LoadScene("End1");
                    break;
                default:
                    GameManager.Instance.LoadScene(remainingTime > 160 ? "EndAccident" : "EndRan");
                    break;
            }
        }
    }
}