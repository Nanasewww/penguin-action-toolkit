using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace PAT
{
    public class DamageNumberUnit : MonoBehaviour
    {
        public Color critialBreakColor;
        public Color critialColor;
        public Color breakColor;
        public Color noBreakColor;

        private Player _player;
        
        float maxlife = 0.75f;
        protected float decayTime = 0.4f;
        float currentlife;
        float speed = 0f;
        Vector3 dir = new Vector3(2, 1, 0);
        TextMeshProUGUI tmp;
        Vector3 pos;
        Vector3 off;

        private void Awake()
        {
            tmp = GetComponent<TextMeshProUGUI>();
        }

        public void Initialization(Player player, Vector3 position, Vector3 offset, EffectModValue mod)
        {
            _player = player;
            
            tmp = GetComponent<TextMeshProUGUI>();

            bool critical = mod.ownerEffect.infoTags.Contains(GamePlayTag.Critical);
            bool breaked = mod.ownerEffect.infoTags.Contains(GamePlayTag.Impact);
            tmp.text = mod.value.ToString();

            if (critical)
            {
                tmp.text += "!";
                tmp.fontSize += 10;
                if (breaked)
                {
                    tmp.color = critialBreakColor;
                }
                else
                {
                    tmp.color = critialColor;
                }
            }
            else
            {
                if (breaked)
                {
                    tmp.color = breakColor;
                }
                else
                {
                    tmp.color = noBreakColor;
                }
            }
            
            //tmp.color = effectsInfo.blockBreak ? breakColor : Color.Lerp(breakColor, noBreakColor, effectsInfo.target.blockRatio);
            currentlife = maxlife;
            this.pos = position;
            off = offset;
        }

        private void Update()
        {
            if(_player  == null) return;
            
            RectTransform rectThis = ((RectTransform)transform);
            RectTransform rectParent = ((RectTransform)transform.parent);
            rectThis.anchoredPosition = _player.WorldToPlayerViewPositionNormalized(pos);
            rectThis.anchoredPosition = new Vector2(rectThis.anchoredPosition.x * rectParent.sizeDelta.x, rectThis.anchoredPosition.y* rectParent.sizeDelta.y);
            rectThis.anchoredPosition += (Vector2)(dir * (speed * (maxlife - currentlife)) + off);

            tmp.enabled = _player.WorldToPlayerViewPositionNormalized(pos).z is > 0 and < 30;
            Color newC = tmp.color;
            if (maxlife - decayTime != 0) newC.a = currentlife / (maxlife - decayTime);
            tmp.color = newC;

            currentlife -= Time.deltaTime;

            if (currentlife < 0) { Destroy(gameObject); }
        }
    }
}
