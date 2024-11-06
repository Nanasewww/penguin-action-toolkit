using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace PAT
{
    public class LocomotionMod : StateModifier
    {
        [SerializeField] protected EffectModValue.ModPattern modPattern = EffectModValue.ModPattern.Override;
        [SerializeField] protected LocomotionData _locomotionData = new LocomotionData();

        protected Effect _effect;
        
        public LocomotionData locomotionData { get { return _locomotionData; } set { _locomotionData = value; } }
        
        public override void BeginEvent()
        {
            base.BeginEvent();
            
            UpdateEffects();
            characterController.AddEffect(_effect);
        }

        protected void UpdateEffects()
        {
            EffectModValue moveMod = ScriptableObject.CreateInstance<EffectModValue>();
            EffectModValue rotateMod = ScriptableObject.CreateInstance<EffectModValue>();
            EffectModValue animateMod = ScriptableObject.CreateInstance<EffectModValue>();
            EffectModValue rootMod = ScriptableObject.CreateInstance<EffectModValue>();
            
            moveMod.modPattern = modPattern;
            rotateMod.modPattern = modPattern;
            animateMod.modPattern = modPattern;
            rootMod.modPattern = modPattern;

            moveMod.resourceTag = GamePlayTag.locoSpeed;
            rotateMod.resourceTag = GamePlayTag.locoRotationSpeed;
            animateMod.resourceTag = GamePlayTag.locoAnimSpeed;
            rootMod.resourceTag = GamePlayTag.locoRootMulti;

            moveMod.value = _locomotionData.MoveSpeed;
            rotateMod.value = _locomotionData.rotationSpeed;
            animateMod.value = _locomotionData.locomotionAnimationSpeed;
            rootMod.value = _locomotionData.rootMotionMutiplier;
            
            _effect = Effect.NewEffect(null,null,null);
            _effect.components.Add(moveMod);
            _effect.components.Add(rotateMod);
            _effect.components.Add(animateMod);
            _effect.components.Add(rootMod);
        }
        
        

        public override void EndEvent()
        {
            base.EndEvent();

            characterController.RemoveEffect(_effect);
        }
    }
}
