using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PAT
{
    public class UnitHealthBar : MonoBehaviour
    {
        [SerializeField] protected Image img;
        [SerializeField] protected Image img2;
        [SerializeField] protected GameObject allGroup;
        [SerializeField] protected GameObject postureGroup;
        [SerializeField] protected Color downColor;
        
        protected Attribute m_Health;
        protected Posture m_posture;
        protected Color o_color;

        protected Player _player;
        private void Awake()
        {
            o_color = img2.color;
        }

        public void Initialization(PATComponent target, Player player)
        {
            m_Health = target.healthAttribute;
            m_posture = (Posture)m_Health.owner.GetAttributeByTag(GamePlayTag.Posture);
            m_Health.OnBaseValueReachMin += OnHealthDeath;
            
            _player = player;
            if(!m_posture) postureGroup.SetActive(false);
        }

        private void Update()
        {
            //Calculate the on screen position
            RectTransform rectThis = ((RectTransform)transform);
            RectTransform rectParent = ((RectTransform)transform.parent);
            rectThis.anchoredPosition = _player.WorldToPlayerViewPositionNormalized(m_Health.transform.position);
            rectThis.anchoredPosition = new Vector2(rectThis.anchoredPosition.x * rectParent.sizeDelta.x, rectThis.anchoredPosition.y* rectParent.sizeDelta.y);
            
            allGroup.SetActive(_player.WorldToPlayerViewPositionNormalized(m_Health.transform.position).z is > 0 and < 30);
            if (m_Health.maxAmount != 0) img.fillAmount = m_Health.currentAmount / m_Health.maxAmount;
            if (m_posture)
            {
                img2.color = o_color;
                if (m_posture.currentStats == Posture.PostureStat.Down) img2.color = downColor;
                img2.fillAmount = m_posture.currentAmount / m_posture.maxAmount;
            }
        }

        void OnHealthDeath()
        {
            gameObject.SetActive(false);
            m_Health.OnBaseValueReachMin -= OnHealthDeath;
        }
    }
}
