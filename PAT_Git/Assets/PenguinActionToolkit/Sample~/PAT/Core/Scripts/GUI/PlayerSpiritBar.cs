using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class PlayerSpiritBar : MonoBehaviour
    {
        [SerializeField]private GameObject[] dashIcons;

        private Attribute dash;

        private void Start()
        {
            dash = Player.Players[0].character.GetAttributeByTag(GamePlayTag.SpAttack);
        }

        private void Update()
        {
            if(!dash) {dash = Player.Players[0].character.GetAttributeByTag(GamePlayTag.SpAttack);return;}

            for (int i = 0; i < dashIcons.Length; i++)
            {
                bool activated = i+1 <= dash.currentAmount;
                dashIcons[i].SetActive(activated);
            }
        }
    }
}
