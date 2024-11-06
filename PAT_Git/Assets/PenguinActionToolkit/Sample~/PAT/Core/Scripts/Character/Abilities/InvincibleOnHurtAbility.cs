using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PAT.AttributeRelatedComponent
{
    public class InvincibleOnHurtAbility : MonoBehaviour
    {
        public Character owner;
        //todo: maybe not health but attribute in general
        public Attribute health;
        [SerializeField] protected float _invincibilityDuration = 1f;
        [SerializeField] protected bool _invincible;
        protected Effect _invincibilityEffect;
        
        public virtual void Awake()
        {
            if (!owner) owner = GetComponent<Character>();
        }

        private void Start()
        {
            health.OnModApplied  += OnHealthHurt;
        }

        private void OnDestroy()
        {
            health.OnModApplied -= OnHealthHurt;
        }

        void OnHealthHurt(EffectModValue mod)
        {
            if (mod.value < 0 && !_invincible)
            {
                StartCoroutine(InvincibileCounter());
            }
        }

        IEnumerator InvincibileCounter()
        {
            _invincible = true;
            _invincibilityEffect = Effect.NewEffect(new List<GamePlayTag> { GamePlayTag.Invincible }, null, null);
            owner.AddEffect(_invincibilityEffect);
            yield return new WaitForSeconds(_invincibilityDuration);
            owner.RemoveEffect(_invincibilityEffect);
            _invincible = false;
        }
    }
}