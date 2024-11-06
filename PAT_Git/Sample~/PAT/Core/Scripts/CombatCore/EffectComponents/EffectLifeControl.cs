using UnityEngine;

namespace PAT
{
    [System.Serializable]
    public class EffectLifeControl: EffectComponent
    {
        [Tooltip("Set this to none zero and it will be removed after timing matched")]
        public float removeAfterTime = 0f;

        [Tooltip("Toggle this, then effect will remove itself immediately after apply")]
        public bool removeOnApply = false;
        
        public override void OnApply(Effect effect)
        {
            if(removeOnApply) {effect.MarkAsRemove();}
        }

        public override void OnTick(Effect effect)
        {
            if (removeAfterTime != 0 && effect.appliedTime >= removeAfterTime ) { effect.MarkAsRemove(); }
        }

        public override void OnRemove(Effect effect)
        {
            
        }
    }
}