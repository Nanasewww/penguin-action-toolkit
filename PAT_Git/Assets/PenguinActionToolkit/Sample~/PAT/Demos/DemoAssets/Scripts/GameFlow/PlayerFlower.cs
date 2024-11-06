using System;
using UnityEngine;

namespace PAT
{
    public class PlayerFlower: MonoBehaviour
    {
        public AiBrain brain;
        private Character character;

        private void Start()
        {
            character = Player.Players[0].character;
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position, character.transform.position) < 10)
            {
                brain.Leader = character;
            }
        }
    }
}