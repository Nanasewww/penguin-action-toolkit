using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    public class SoftLockMod: StateModifier
    {
        [SerializeField] private float distanceRange = 5f;
        [SerializeField] private float angleRange = 180f;
        private AimAbility aim;

        public override void Initialization(Character controller)
        {
            base.Initialization(controller);
            aim = controller.GetComponent<AimAbility>();
        }

        public override void BeginEvent()
        {
            base.BeginEvent();
            if(aim == null) {Debug.Log("No Aim Ability in Character, mod may not work."); return;}
            
            aim.AttemptSoftLock(distanceRange, angleRange);
        }

        public override void EndEvent()
        {
            base.EndEvent();
            if(aim == null) return;
            
            aim.EndSoftLock();
        }
    }
}