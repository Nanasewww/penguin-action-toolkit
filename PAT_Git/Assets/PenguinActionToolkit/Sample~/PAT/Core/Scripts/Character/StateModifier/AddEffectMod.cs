using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class AddEffectMod : StateModifier
    {
        [SerializeField] protected List<Effect> effects;
        [SerializeField] protected bool removeOnExit;

        private List<Effect> actualEffects;
        public override void BeginEvent()
        {
            base.BeginEvent();
            actualEffects = new List<Effect>();
            foreach (Effect effect in effects)
            {
                Effect newEffect = Instantiate(effect);
                actualEffects.Add(newEffect);
                characterController.AddEffect(newEffect);
            }
        }

        public override void EndEvent()
        {
            base.EndEvent();
            if (removeOnExit) foreach (Effect effect in actualEffects) { effect.MarkAsRemove(); }
        }


    }
}
