using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PAT
{
    public class PlayerMoveDisplay : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI tmp;
        private Character playerCharacter;

        private void Awake()
        {
            if (!tmp) tmp = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            playerCharacter = Player.Players[0].character;
        }

        private void Update()
        {
            if(!tmp) {Debug.LogWarning("no tmp set for move display"); return;}
            if(!playerCharacter) {Debug.LogWarning("no player character detected"); return;}

            string content = "Player Moves \n";
            
            List<ActionState> possibleActions = playerCharacter.GetAllPossibleActions();
            foreach (ActionState action in possibleActions)
            {
                if(action.inputTag == GamePlayTag.None) continue;
                content += "    " + action.inputTag + ": " + action.gameObject.name +"\n";
            }
            
            tmp.text = content;
        }
    }
}
