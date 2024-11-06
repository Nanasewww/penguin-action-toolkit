using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine;

namespace PAT
{
    [Serializable]
    public struct MontageSpeedParameters
    {
        public int beginIndex;
        public int endIndex;
        public float speed;
    }
    public class MeleeCombinedModifier : CombinedModifier
    {
        
        private LocomotionMod locomotionMF;
        private AttackWithHitboxMod _hitboxWithHitboxMod;
        private AnimationMontageMod animationMF;
        private SelfKnockbackMod _selfKnockbackMod;
        private List<MontageSpeedMod> montageSpeedMfs;
        
        // hitbox parameters
        [FormerlySerializedAs("meleeEffectCollider")]
        [Header("HitBox")]
        [SerializeField] private Hitbox meleeHitbox;
        [SerializeField] private int meleeDamage;
        /*public ModifierMode mode;
        [SerializeField] protected int _beginIndex = 1;
        [SerializeField] protected int _endIndex = 2;
        [SerializeField] protected float _beginTime = 0;
        [SerializeField] protected float _endTime = -1;*/
        
        // animation parameters
        [Header("Animation")] 
        [SerializeField] private float animationEndTime = -1;
        [SerializeField] private AnimationInfo animationInfo;
        [SerializeField] private bool exitOnEnd;
        
        // animation montage parameters
        [Header("Animation Montage Speed")] 
        [SerializeField] private List<MontageSpeedParameters> speedParameters;
        
        // locomotion parameters
        [Header("Locomotion")]
        [SerializeField] private float locomotionEndTime = -1;
        [SerializeField] private LocomotionData locomotionData;

        // self knockback parameters
        [Header("Self-KnockBack")]
        [SerializeField] private bool selfKnockBack;
        [SerializeField] private float knockBackValue;
        [SerializeField] private int knockBackLevel;
        [SerializeField] private GamePlayTag knockBackTag;
        
        /*
        #region Getter
        public int beginIndex { get { return _beginIndex; } set { _beginIndex = value; } }
        public int endIndex { get { return _endIndex; } set { _endIndex = value; } }
        public float beginTime { get { return _beginTime; } set { _beginTime = value; } }
        public float endTime { get { return _endTime; } set { _endTime = value; } }
        #endregion
        */

        public void ModifierFactory()
        {
            // child object initialize
            GameObject childModifier = new GameObject("MeleeModifiers");
            childModifier.transform.parent = transform;
            locomotionMF = childModifier.AddComponent<LocomotionMod>();
            _hitboxWithHitboxMod = childModifier.AddComponent<AttackWithHitboxMod>();
            animationMF = childModifier.AddComponent<AnimationMontageMod>();
            if (selfKnockBack) _selfKnockbackMod = childModifier.AddComponent<SelfKnockbackMod>();
            montageSpeedMfs = new List<MontageSpeedMod>();
            foreach (MontageSpeedParameters speedParameters in speedParameters)
            {
                montageSpeedMfs.Add(childModifier.AddComponent<MontageSpeedMod>());
            }
            
            
            // hitbox initialize
            _hitboxWithHitboxMod.owningHitbox = meleeHitbox;
            _hitboxWithHitboxMod._factory = new EffectFactory_P();
            ((EffectFactory_P)_hitboxWithHitboxMod._factory).damage = meleeDamage;
            _hitboxWithHitboxMod.mode = mode;
            _hitboxWithHitboxMod.beginIndex = _beginIndex;
            _hitboxWithHitboxMod.endIndex = _endIndex;
            _hitboxWithHitboxMod.beginTime = _beginTime;
            _hitboxWithHitboxMod.endTime = _endTime;
            
            // animation initialize
            animationMF.mode = ModifierMode.ByTimeInState;
            animationMF.endTime = animationEndTime;
            animationMF.info = animationInfo;
            animationMF.exitOnMontageEnd = exitOnEnd;
            
            // montage speed initialize
            for (int i = 0; i < speedParameters.Count; i++)
            {
                montageSpeedMfs[i].mode = ModifierMode.ByAnimationEvent;
                montageSpeedMfs[i].beginIndex = speedParameters[i].beginIndex;
                montageSpeedMfs[i].endIndex = speedParameters[i].endIndex;
                montageSpeedMfs[i].speed = speedParameters[i].speed;
            }
            
            // locomotion initialize
            locomotionMF.mode = ModifierMode.ByTimeInState;
            locomotionMF.endTime = locomotionEndTime;
            locomotionMF.locomotionData = locomotionData;
            
            // self-knockback initialize
            if (selfKnockBack)
            {
                _selfKnockbackMod.effectValue = knockBackValue;
                _selfKnockbackMod.effectLevel = knockBackLevel;
                _selfKnockbackMod.resourceTag = knockBackTag;
                _selfKnockbackMod.mode = ModifierMode.ByTimeInState;
            }
            
            // add everything to stateModifiers
            stateModifiers.Add(locomotionMF);
            stateModifiers.Add(_hitboxWithHitboxMod);
            stateModifiers.Add(animationMF);
            foreach (MontageSpeedMod montageSpeedMf in montageSpeedMfs)
            {
                stateModifiers.Add(montageSpeedMf);
            }
            if (selfKnockBack) stateModifiers.Add(_selfKnockbackMod);
        }
        
        public override void Initialization(Character controller)
        {
            ModifierFactory();
            base.Initialization(controller);
        }
    }
}