using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace PAT
{
    public class AimIKtoTargetMod: StateModifier
    {
        public float aimDisRange = 20f;
        public float aimAngleRange = 180f;
        public float weight = 1;
        public float minLimit = -180;
        public float maxLimit = 180;

        private MultiAimConstraint _constraint;
        private Transform targetObj;
        private Transform aimAssitObj;
        private AimAbility aim;
        
        public override void Initialization(Character controller)
        {
            base.Initialization(controller);
            _constraint = controller.modelHandler.GetComponentInChildren<MultiAimConstraint>();
            aim = controller.GetComponent<AimAbility>();

        }

        public override void BeginEvent()
        {
            base.BeginEvent();
            if(aim == null) {Debug.Log("No Aim Ability in Character, mod may not work."); return;}
            if(!_constraint){Debug.Log("No Multi Aim Constrain found, may not work properly"); return;}
            
            aimAssitObj = _constraint.data.sourceObjects[0].transform;
            _constraint.data.limits = new Vector2(minLimit, maxLimit);
            
            aim.AttemptSoftLock(aimDisRange, aimAngleRange);
            targetObj = aim.GetCurrentLockTarget();
            
            if(targetObj) _constraint.weight = weight;

        }

        public override void EarlyOnUpdate(Character controller)
        {
            base.EarlyOnUpdate(controller);
            if(!aim) return;
            if(!_constraint) return;

            if (!targetObj)
            {
                aim.AttemptSoftLock(aimDisRange, aimAngleRange);
                targetObj = aim.GetCurrentLockTarget();
            }
            
            if(targetObj) aimAssitObj.position = targetObj.position;
        }

        public override void EndEvent()
        {
            base.EndEvent();
            
            if(!aim) return;
            if(!_constraint) return;

            aim.EndSoftLock();
            _constraint.weight = 0;
            aimAssitObj.localPosition = Vector3.zero;
        }
    }
}