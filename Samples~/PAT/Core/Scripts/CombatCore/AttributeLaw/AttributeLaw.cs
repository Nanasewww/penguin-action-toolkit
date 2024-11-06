using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    /// <summary>
    /// This object controls how attribute will perform when mod applied
    /// Put the scriptable objects in the "Resource/AttributeLaw" folder to make them global
    /// Or you apply them to character/effect to make them local
    /// Remember that you need add your own asset menu as well
    ///
    /// You might apply multiple law to the same Tag
    /// But keep in mind that the order might not be consistent
    /// So try to make them not overlaping
    /// </summary>
    public abstract class AttributeLaw : ScriptableObject, IComparable<AttributeLaw>
    {
        public static List<AttributeLaw> allInstances;
        public int order = 0;
        [Tooltip("This law is only applying to attribute ")]
        [SerializeField] protected GamePlayTag _resourceTag = GamePlayTag.None;
        
        [Header("The following tags are looking for tags in effect's info tags, not character")]
        [Tooltip("Need this tag to taken effect, unless it's none")]
        [SerializeField] protected GamePlayTag _requiredTag = GamePlayTag.None;
        [Tooltip("Would not be effect if this tag exist, unless it's none")]
        [SerializeField] protected GamePlayTag _prohibitTag = GamePlayTag.None;
        
        [Header("This law only applies to mod of the same setting")]
        [SerializeField] protected EffectModValue.ModTarget _modTarget = EffectModValue.ModTarget.Amount;
        [SerializeField] protected EffectModValue.ModPattern _modPattern = EffectModValue.ModPattern.ToBaseValue;
        [SerializeField] protected ValueType _targetValueType = ValueType.LessThanOrEqualToZero;
        protected enum ValueType
        {
            LessThanZero,
            LessThanOrEqualToZero,
            Zero,
            GreaterThanOrEqualToZero,
            GreaterThanZero,
        }
        
        public GamePlayTag resourceTag { get => _resourceTag; }
        
        private void OnDestroy()
        {
            allInstances.Remove(this);
        }

        [ContextMenu("Log")]
        void Log()
        {
            if(allInstances == null) {InitializeList();}
            if(allInstances == null) {return;}
            
            Debug.Log(string.Join(", ", allInstances)+ " is loaded as AttributeLaw");
        }

        static void InitializeList()
        {
            if(allInstances == null)allInstances = new List<AttributeLaw>();
            AttributeLaw[] loadedInstances = Resources.LoadAll<AttributeLaw>("GlobalAttributeLaw");
            if(loadedInstances != null) allInstances = loadedInstances.ToList();
        }

        public static List<AttributeLaw> GetGlobalLaw(GamePlayTag tag)
        {
            if(allInstances == null) {InitializeList();}
            if(allInstances == null) {return null;}
            
            if(allInstances.Count <= 0) return null;

            List<AttributeLaw> toReturn = new List<AttributeLaw>();
            foreach (AttributeLaw attributeLaw in allInstances)
            {
                if(attributeLaw._resourceTag == tag) toReturn.Add(attributeLaw);
            }
            return toReturn;
        }

        public abstract EffectModValue ApplyLawToMod(Attribute attribute, EffectModValue mod);

        /// <summary>
        /// Please always use this
        /// We might need to record law actually applied for better debug
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        public virtual bool CanApplyLaw(Attribute attribute, EffectModValue mod)
        {
            if (attribute.resourceTag != resourceTag) return false;
            if (mod.ownerEffect.infoTags.Contains(_prohibitTag)) return false;
            if (_requiredTag != GamePlayTag.None && !mod.ownerEffect.infoTags.Contains(_requiredTag)) return false;
            if (mod.modTarget != _modTarget) return false;
            if (mod.modPattern != _modPattern) return false;

            switch (_targetValueType)
            {
                case ValueType.LessThanZero:
                    if(mod.value >= 0) return false;
                    break;
                case ValueType.LessThanOrEqualToZero:
                    if(mod.value > 0) return false;
                    break;
                case ValueType.Zero:
                    if(mod.value != 0) return false;
                    break;
                case ValueType.GreaterThanOrEqualToZero:
                    if(mod.value < 0) return false;
                    break;
                case ValueType.GreaterThanZero:
                    if(mod.value <= 0) return false;
                    break;
            }
            
            return true;
        }
        public int CompareTo(AttributeLaw other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            return order.CompareTo(other.order);
        }
    }
}
