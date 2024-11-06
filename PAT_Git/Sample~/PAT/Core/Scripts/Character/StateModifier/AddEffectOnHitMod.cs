using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class AddEffectOnHitMod: OnEffectSentMod
    {
        public bool applyToSelf = true;
        public List<Effect> toAdd;
        
        private List<Effect> actualEffects;

        public override void BeginEvent()
        {
            base.BeginEvent();
            actualEffects = new List<Effect>();
            foreach (var effect in toAdd)
            {
                actualEffects.Add(Instantiate(effect));
            }
        }

        protected override void OnHit(PATComponent.EffectPackage info)
        {
            base.OnHit(info);

            foreach (Effect effect in actualEffects)
            {
                if(applyToSelf) characterController.AddEffect(effect);
                else info.target?.AddEffect(effect);
            }
        }
    }
}