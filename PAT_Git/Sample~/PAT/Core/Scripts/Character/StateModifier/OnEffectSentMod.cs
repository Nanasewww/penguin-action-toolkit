using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace PAT
{
    public class OnEffectSentMod: StateModifier
    {
        public UnityEvent<PATComponent.EffectPackage> onHitEvent;


        public override void Initialization(Character controller)
        {
            base.Initialization(controller);
            if (onHitEvent == null) onHitEvent = new UnityEvent<PATComponent.EffectPackage>();
        }

        protected virtual void OnHit(PATComponent.EffectPackage info)
        {
            onHitEvent.Invoke(info);
        }

        public override void BeginEvent()
        {
            base.BeginEvent();
            characterController.onEffectPackageSent += OnHit;

        }
        
        public override void EndEvent()
        {
            base.EndEvent();
            characterController.onEffectPackageSent -= OnHit;
        }
    }
}