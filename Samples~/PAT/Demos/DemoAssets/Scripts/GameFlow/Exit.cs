using System;
using UnityEngine;

namespace PAT
{
    public class Exit: MonoBehaviour
    {

        protected LevelManager _manager;

        private void Awake()
        {
            _manager = FindObjectOfType<LevelManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Character c = other.GetComponent<Character>();
            if(!c) return;
            if (c == Player.Players[0].character)
            {
                _manager.EndGame();
            }
            Debug.Log(c.gameObject.name);
        }
    }
}