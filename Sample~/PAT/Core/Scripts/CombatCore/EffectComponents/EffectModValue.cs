using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    public class EffectModValue: EffectComponent
    {
        public enum ModPattern
        {
            ToBaseValue = 0,
            Add = 1,
            AddMulti = 2,
            FinalMulti = 3,
            Override = 4
        }

        public enum ModTarget
        {
            Amount = 0,
            RecoverRate = 1
        }
        
        public GamePlayTag resourceTag = GamePlayTag.None;
        public ModTarget modTarget = ModTarget.Amount;
        public ModPattern modPattern = ModPattern.ToBaseValue;
        public float value;
        public int order = 0;
        [Tooltip("If you toggle this on, this mod will ignore any attribute Law when applied")]
        public bool ignoreLaw = false;
        
        private Attribute targetAttribute;
        private Effect _ownerEffect;
        
        public Effect ownerEffect {get{return _ownerEffect;}}
        
        public override void OnApply(Effect effect)
        {
            _ownerEffect = effect;
            targetAttribute = effect.target.GetAttributeByTag(resourceTag);
            if(targetAttribute) targetAttribute.AddEffectModValue(this);
        }

        public override void OnTick(Effect effect)
        {

        }

        public override void OnRemove(Effect effect)
        {
            if(targetAttribute) targetAttribute.RemoveEffectModValue(this);
        }
    }
}