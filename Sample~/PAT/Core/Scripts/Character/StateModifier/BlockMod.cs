using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class BlockMod : StateModifier
    {
        [SerializeField] protected float _blockRatio;
        [SerializeField] protected float _knockBlockRatio;
        [SerializeField] protected int _resistence;

        protected Effect blockEffect;
        protected EffectModValue dmgBlockMod;
        protected EffectModValue knockBlockMod;
        protected EffectModValue resistMod;


        private void Awake()
        {
            
            dmgBlockMod = ScriptableObject.CreateInstance<EffectModValue>();
            knockBlockMod = ScriptableObject.CreateInstance<EffectModValue>();
            resistMod = ScriptableObject.CreateInstance<EffectModValue>();

            List<EffectComponent> components = new List<EffectComponent> { dmgBlockMod, knockBlockMod, resistMod };
            
            blockEffect = Effect.NewEffect(null, components , characterController);
            blockEffect.components.Add(dmgBlockMod);
            blockEffect.components.Add(knockBlockMod);
            blockEffect.components.Add(resistMod);
            
            dmgBlockMod.resourceTag = GamePlayTag.damageReduce;
            knockBlockMod.resourceTag = GamePlayTag.knockBackReduce;
            resistMod.resourceTag = GamePlayTag.Resistance;
            
            dmgBlockMod.modTarget = EffectModValue.ModTarget.Amount;
            knockBlockMod.modTarget = EffectModValue.ModTarget.Amount;
            resistMod.modTarget = EffectModValue.ModTarget.Amount;

            dmgBlockMod.modPattern = EffectModValue.ModPattern.Override;
            knockBlockMod.modPattern = EffectModValue.ModPattern.Override;
            resistMod.modPattern = EffectModValue.ModPattern.Override;
        }

        public override void BeginEvent()
        {
            base.BeginEvent();

            //Those value needs to be update every time
            dmgBlockMod.value = _blockRatio;
            knockBlockMod.value = _knockBlockRatio;
            resistMod.value = _resistence;
            
            characterController.AddEffect(blockEffect);
        }

        public override void EndEvent()
        {
            base.EndEvent();
            characterController.RemoveEffect(blockEffect);
        }
    }
}
