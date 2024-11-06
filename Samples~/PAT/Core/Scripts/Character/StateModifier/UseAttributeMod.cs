using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PAT
{
    public class UseAttributeMod: StateModifier
    {
        public GamePlayTag resourceTag;
        public float amount;
        public bool canOverUse;
        public bool banRecovery = true;
        
        protected Attribute mAttribute;
        protected Effect banEffect;
        protected EffectModValue _valueMod;
        public override void Initialization(Character controller)
        {
            base.Initialization(controller);
            mAttribute = controller.GetAttributeByTag(resourceTag);
            
            if(!mAttribute)Debug.LogWarning("No " + resourceTag +" found in this character");

            _valueMod = ScriptableObject.CreateInstance<EffectModValue>();
            _valueMod.resourceTag = resourceTag;
            _valueMod.modPattern = EffectModValue.ModPattern.Override;
            _valueMod.modTarget = EffectModValue.ModTarget.RecoverRate;
            _valueMod.value = 0;
            
            banEffect = Effect.NewEffect(null, null , controller);
            banEffect.components.Add(_valueMod);//This shall work as well

        }

        public override bool Validate(Character controller)
        {
            if (!mAttribute) return true; 
            
            if (mAttribute.currentAmount < amount && !canOverUse) return false;

            if (canOverUse && mAttribute.currentAmount <= 0) return false;
            
            return base.Validate(controller);
        }

        public override void BeginEvent()
        {
            base.BeginEvent();
            if(!mAttribute) return;
             
            mAttribute.ModBaseAttribute(-amount);
            if (banRecovery) characterController.AddEffect(banEffect);

        }

        public override void EndEvent()
        {
            base.EndEvent();
            if (banRecovery) characterController.RemoveEffect(banEffect);
        }
    }
}