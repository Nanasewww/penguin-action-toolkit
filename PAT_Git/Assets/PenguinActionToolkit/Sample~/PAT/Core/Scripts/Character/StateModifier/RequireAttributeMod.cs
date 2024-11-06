using System.Collections;
using System.Collections.Generic;
using PAT;
using Unity.VisualScripting;
using UnityEngine;

namespace PAT
{
    public enum RequireType
    {
        Equal,
        GreaterEqual,
        Greater,
        LessEqual,
        Less
    }
    public class RequireAttributeMod : StateModifier
    {
        public GamePlayTag resourceTag;
        public float amount;
        public RequireType requireType;
        protected Attribute mAttribute;
        public override void Initialization(Character controller)
        {
            base.Initialization(controller);
            mAttribute = controller.GetAttributeByTag(resourceTag);

            if (!mAttribute) Debug.LogWarning("No " + resourceTag + " found in this character");
        }

        public override bool Validate(Character controller)
        {
            if (!mAttribute) return false;

            switch (requireType)
            {
                case RequireType.Equal:
                    if (mAttribute.currentAmount != amount) return false;
                    break;
                case RequireType.GreaterEqual:
                    if (mAttribute.currentAmount < amount) return false;
                    break;
                case RequireType.Greater:
                    if (mAttribute.currentAmount <= amount) return false;
                    break;
                case RequireType.LessEqual:
                    if (mAttribute.currentAmount > amount) return false;
                    break;
                case RequireType.Less:
                    if (mAttribute.currentAmount >= amount) return false;
                    break;
            }
            
            return base.Validate(controller);
        }
    }
}
