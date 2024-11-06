using System;
using UnityEngine;

namespace PAT
{
    public class JumpCountMod: StateModifier
    {
        [SerializeField] private int maxExtraJump = 1;
        [SerializeField] private int currentJump = 0;
        
        public override bool Validate(Character controller)
        {
            bool result = true;
            if (!controller.Locomotion.onGround && currentJump < maxExtraJump) result = true;
            else result = false;
            
            return result && base.Validate(controller);
        }

        public override void OnEnter(Character controller)
        {
            base.OnEnter(controller);
            ++currentJump;
        }

        public void Update()
        {
            if (timeInState > 0 && characterController.Locomotion.onGround) 
                currentJump = 0;
        }
    }
}