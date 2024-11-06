using System;
using UnityEngine;
using UnityEngine.UI;

namespace PAT
{
    public class PlayerAttributeDrawer: MonoBehaviour
    {
        public int playerID = -1;
        public GamePlayTag resourceTag = GamePlayTag.Health;
        [SerializeField] protected Image img;
        protected Attribute attribute;

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
            
            img.fillAmount = attribute.currentAmount / attribute.maxAmount;
            
        }
        
    }
}