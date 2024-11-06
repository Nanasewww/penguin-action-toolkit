using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PAT
{
    [CreateAssetMenu(menuName = "PAT/AttributeLaw/AddInfoTagLaw")]
    public class CheckForHigherAttribute: AttributeLaw
    {
        [Serializable]
        public class CompareUnit
        {
            public enum UnitType
            {
                Attribute,
                EffectValue,
                StaticFloat,
                RandomNumber
            }

            public UnitType type;
            public GamePlayTag attributeTag = GamePlayTag.None;
            public float staticFloat;
            public float randomMin;
            public float randomMax;

            PATComponent component;
            
            public bool isValid(PATComponent pat)
            {
                if (pat == null) return false;
                component = pat;

                if(type == UnitType.Attribute && component.GetAttributeByTag(attributeTag) == null) return false;

                return true;
            }

            public float GetValue(EffectModValue mod)
            {
                switch (type)
                {
                    case UnitType.Attribute:
                        return component.GetAttributeByTag(attributeTag).currentAmount;
                    case UnitType.EffectValue:
                        return mod.value;
                    case UnitType.RandomNumber:
                        return Random.Range(randomMin, randomMax);
                    default:
                        return staticFloat;
                }
            }
        }
        
        [Header("Specific Settings")]
        public CompareUnit sourceUnit;
        public CompareUnit targetUnit;
        public GamePlayTag infoTagOnSourceHigher = GamePlayTag.None;
        public GamePlayTag infoTagOnEqual = GamePlayTag.None;
        public GamePlayTag infoTagOnTargetHigher = GamePlayTag.None;


        public override bool CanApplyLaw(Attribute attribute, EffectModValue mod)
        {
            //todo: shall this be like that?
            if (mod.ownerEffect.source == null) return false;
            if (mod.ownerEffect.target == null) return false;
            if (!sourceUnit.isValid(mod.ownerEffect.source)) return false;
            if (!targetUnit.isValid(mod.ownerEffect.target)) return false;
            
            return base.CanApplyLaw(attribute, mod);
        }

        public override EffectModValue ApplyLawToMod(Attribute attribute, EffectModValue mod)
        {
            if (sourceUnit.GetValue(mod) > targetUnit.GetValue(mod))
            {
                if(infoTagOnSourceHigher != GamePlayTag.None) mod.ownerEffect.infoTags.Add(infoTagOnSourceHigher);
            }
            else if (sourceUnit.GetValue(mod) < targetUnit.GetValue(mod))
            {
                if(infoTagOnTargetHigher != GamePlayTag.None) mod.ownerEffect.infoTags.Add(infoTagOnTargetHigher);
            }
            else
            {
                if(infoTagOnEqual != GamePlayTag.None) mod.ownerEffect.infoTags.Add(infoTagOnEqual);
            }
            
            return mod;
        }
    }
}