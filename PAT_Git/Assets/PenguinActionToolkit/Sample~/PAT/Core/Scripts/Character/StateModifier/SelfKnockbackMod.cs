using System;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class SelfKnockbackMod : OnEffectSentMod
    {
        public GamePlayTag resourceTag;
        public float effectValue;
        public int effectLevel;
        private Effect knockBackEffect;
        private EffectKnockBack _knockBack;
        private EffectLifeControl _lifeControl;
        public override void OnEnter(Character controller)
        {
            base.OnEnter(controller);
            _knockBack = ScriptableObject.CreateInstance<EffectKnockBack>();
            _lifeControl = ScriptableObject.CreateInstance<EffectLifeControl>();
            _lifeControl.removeOnApply = true;
            
            knockBackEffect = Effect.NewEffect(null, new List<EffectComponent> { _knockBack, _lifeControl },
                controller);

        }

        protected override void OnHit(PATComponent.EffectPackage info)
        {
            _knockBack.knockBack = this.effectValue;
            characterController.AddEffect(knockBackEffect);
            base.OnHit(info);
        }
    }
}