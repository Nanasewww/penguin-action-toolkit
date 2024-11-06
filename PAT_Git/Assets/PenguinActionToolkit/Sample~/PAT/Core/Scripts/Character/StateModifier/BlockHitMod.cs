using UnityEngine;
using UnityEngine.Events;

namespace PAT
{
    public class BlockHitMod: StateModifier
    {
        public UnityEvent<PATComponent.EffectPackage> onBlockHitEvent;
        [SerializeField] protected ActionState changeStateOnHit;
        [SerializeField] protected GamePlayTag triggerStateOnHit = GamePlayTag.None;

        void OnHit(PATComponent.EffectPackage package)
        {
            bool blockHit = false;

            foreach (var effect in package.effects)
            {
                if (effect.infoTags.Contains(GamePlayTag.Block))
                {
                    blockHit = true;
                    break;
                }
            }
            if(!blockHit) return;
            
            if (changeStateOnHit) characterController.ChangeState(changeStateOnHit);
            if (triggerStateOnHit != GamePlayTag.None) characterController.TriggerStateWithTag(triggerStateOnHit);
            onBlockHitEvent.Invoke(package);
        }

        public override void BeginEvent()
        {
            base.BeginEvent();
            characterController.onEffectPackageReceived += OnHit;
        }
        
        public override void EndEvent()
        {
            base.EndEvent();
            characterController.onEffectPackageReceived -= OnHit;
        }
    }
}