using UnityEngine;

namespace PAT
{
    public class MontageSpeedMod: StateModifier
    {
        [SerializeField] protected float _speed = 1;
        [SerializeField] protected EffectModValue.ModPattern modPattern = EffectModValue.ModPattern.FinalMulti;
        
        protected Effect _effect;

        public float speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public override void BeginEvent()
        {
            base.BeginEvent();
            UpdateEffects();
            characterController.AddEffect(_effect);
        }
        
        protected void UpdateEffects()
        {
            EffectModValue montageSpeedMod = ScriptableObject.CreateInstance<EffectModValue>();
            montageSpeedMod.modPattern = modPattern;
            montageSpeedMod.resourceTag = GamePlayTag.ActionSpeed;
            montageSpeedMod.value = _speed;
            _effect = Effect.NewEffect(null,null,null);
            _effect.components.Add(montageSpeedMod);
            
        }

        public override void EndEvent()
        {
            base.EndEvent();
            characterController.RemoveEffect(_effect);
        }
    }
}