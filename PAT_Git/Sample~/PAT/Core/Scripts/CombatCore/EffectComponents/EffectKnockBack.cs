using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    /// <summary>
    /// Since this require state changing and knockback
    /// This will only work when the target is a Character
    /// </summary>
    public class EffectKnockBack: EffectComponent
    {
        public float knockBack = 1.0f;
        [Tooltip("KnockBack will fully impact if this tag is true in effect, or it will be reduced")]
        public GamePlayTag impactTag = GamePlayTag.Impact;
        [Tooltip("If knockBack shall be reduce, find the attribute in target that represent reduction amount")]
        public GamePlayTag reduceTag = GamePlayTag.knockBackReduce;
        public override void OnApply(Effect effect)
        {
            if(!(effect.target is Character character)) return;
            if(!character) return;
            
            bool penetratedResistance = effect.infoTags.Contains(impactTag);
            Attribute reduce = character.GetAttributeByTag(reduceTag);

            if(penetratedResistance){character?.TriggerStateWithTag(GamePlayTag.Hurt);}
            
            float knockBackRatio = 1;
            
            //The level of effect represents Impact level
            if(!penetratedResistance && reduce != null)
            {
                knockBackRatio -= reduce.currentAmount;
            }
            
            Locomotion3DForce f = new Locomotion3DForce();

            Vector3 dir;
            
            //calculate direction base on dmg source
            if(effect.source){ dir = effect.source.transform.forward;}
            else{ dir = -character.transform.forward;}

            f.dir = dir.normalized;
            f.magnitude = knockBack * knockBackRatio * 8f;
            f.decaySpeed = knockBack * knockBackRatio * 8f/ 0.2f;
            
            //owner.Locomotion.RotateToward(-dir);
            character.Locomotion.AddForce(f);
        }

        public override void OnTick(Effect effect)
        {
            
        }

        public override void OnRemove(Effect effect)
        {
            
        }
    }
}