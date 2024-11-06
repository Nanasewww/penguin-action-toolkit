using UnityEngine;

namespace PAT
{
    [CreateAssetMenu(menuName = "PAT/AttributeLaw/ModByAttributeLaw")]
    public class ModBySourceAtAttributeLaw: AttributeLaw
    {
        public enum AttributeFrom
        {
            Source,
            Target
        }

        public enum Calculation
        {
            Add,
            Subtract,
            Multi,
            Divide,
            RatioReduce,
        }
        [Header("Mod By Attribute Law Settings")]
        [Tooltip("what attribute should be used to influence this")]
        public GamePlayTag otherAttributeTag = GamePlayTag.None;
        public AttributeFrom attributeFrom = AttributeFrom.Source;
        [Tooltip("Ratio reduce means multi by 1-x, x should be between 0 and 1")]
        public Calculation calculation = Calculation.Add;

        public override bool CanApplyLaw(Attribute attribute, EffectModValue mod)
        {
            PATComponent component = mod.ownerEffect.source;
            if(attributeFrom == AttributeFrom.Target) component = mod.ownerEffect.target;
            
            if(!component) return false;//if no source or target
            if (component.GetAttributeByTag(otherAttributeTag) == null) return false;//if no attribute found
            
            return base.CanApplyLaw(attribute, mod);
        }

        public override EffectModValue ApplyLawToMod(Attribute attribute, EffectModValue mod)
        {
            //Get the attribute, as we validated this should work
            PATComponent component = mod.ownerEffect.source;
            if(attributeFrom == AttributeFrom.Target) component = mod.ownerEffect.target;
            Attribute otherAttribute = component.GetAttributeByTag(otherAttributeTag);

            switch (calculation)
            {
                case Calculation.Add:
                    mod.value += otherAttribute.currentAmount;
                    break;
                case Calculation.Subtract:
                    mod.value -= otherAttribute.currentAmount;
                    break;
                case Calculation.Multi:
                    mod.value *= otherAttribute.currentAmount;
                    break;
                case Calculation.Divide:
                    mod.value /= otherAttribute.currentAmount;
                    break;
                case Calculation.RatioReduce:
                    mod.value *= 1-otherAttribute.currentAmount;
                    break;
            }

            return mod;
        }
    }
}