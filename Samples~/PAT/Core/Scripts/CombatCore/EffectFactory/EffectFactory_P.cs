using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace PAT
{
    [Serializable]
    [MovedFrom(false,null,null,"Damage_PAT")]
    public class EffectFactory_P: EffectFactory
    {
        public GamePlayTag atkTag = GamePlayTag.Attack;
        public float damage;
        public float postureDamage;
        public float impactLevel;
        public float knockBack;
        public List<GamePlayTag> mainTagsForEffect;
        public override List<Effect> GenerateEffect(PATComponent sourceComponent)
        {
            List<Effect> effects = new List<Effect>();

            EffectModValue impactMod = ScriptableObject.CreateInstance<EffectModValue>();
            impactMod.value = impactLevel;
            impactMod.resourceTag = GamePlayTag.Resistance;
            
            EffectModValue dmgMod = ScriptableObject.CreateInstance<EffectModValue>();
            dmgMod.value = - damage;
            dmgMod.resourceTag = GamePlayTag.Health;
            
            EffectModValue postureMod = ScriptableObject.CreateInstance<EffectModValue>();
            postureMod.value = - postureDamage;
            postureMod.resourceTag = GamePlayTag.Posture;
            
            EffectKnockBack knockBackMod = ScriptableObject.CreateInstance<EffectKnockBack>();
            knockBackMod.knockBack= knockBack;
            knockBackMod.impactTag = GamePlayTag.Impact;
            knockBackMod.reduceTag = GamePlayTag.knockBackReduce;
            
            EffectLifeControl lifeControl = ScriptableObject.CreateInstance<EffectLifeControl>();
            lifeControl.removeOnApply = true;

            //The order of mods matters
            Effect effect = Effect.NewEffect(null,
                new List<EffectComponent> { impactMod, dmgMod, knockBackMod, postureMod, lifeControl }, sourceComponent);
            effects.Add(effect);
            return effects;
        }
        
    }
}