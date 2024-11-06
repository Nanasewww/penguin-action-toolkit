using System;
using UnityEngine;

namespace PAT
{
    public class PlayerAttributeDrawerIcon: MonoBehaviour
    {
        public int playerID = -1;
        public GamePlayTag resourceTag = GamePlayTag.Health;
        [SerializeField]private GameObject[] icons;

        private Attribute attribute;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            attribute = Player.GetPlayerByID(playerID)?
                .character.GetAttributeByTag(resourceTag);
        }

        private void Update()
        {
            if(!attribute) {Initialize();return;}

            for (int i = 0; i < icons.Length; i++)
            {
                bool activated = i+1 <= attribute.currentAmount;
                icons[i].SetActive(activated);
            }
        }
    }
}