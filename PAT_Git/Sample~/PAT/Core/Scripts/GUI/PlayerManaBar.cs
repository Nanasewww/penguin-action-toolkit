using UnityEngine;
using UnityEngine.UI;

namespace PAT
{
    public class PlayerManaBar: MonoBehaviour
    {
        [SerializeField] protected Image img;
        protected Attribute mana;

        private void Start()
        {
            mana = Player.Players[0].character.GetAttributeByTag(GamePlayTag.Mana);
        }

        private void Update()
        {
            if(!mana) return;
            
            img.fillAmount = mana.currentAmount / mana.maxAmount;
        }
    }
}