using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class PlayerJump : ActionState
    {
        [SerializeField] protected float jumpHeight;
        [SerializeField] protected float jumpSpeed;
        [SerializeField] protected float jumpEndSpeed;

        protected float targetJumpTime;
        protected bool rising = false;
        
        private void Reset()
        {
            _mainTags = new List<GamePlayTag> { GamePlayTag.Jumping, GamePlayTag.Idle };


            jumpHeight = 5;
            jumpSpeed = 20;
            jumpEndSpeed = 5;

            
            // _animationInfo.stateName = "Jump";
        }
        public override bool Validation()
        {
            if (!character.Locomotion.onGround) return false;

            return base.Validation();
        }
        public override void OnEnter()
        {
            base.OnEnter();
            
            targetJumpTime = jumpHeight / jumpSpeed;
            rising = true;
        }

        public override void OnTick(float deltaTime)    
        {
            base.OnTick(deltaTime);
            
            if (timeInState >= targetJumpTime && rising)
            {
                //Debug.Log("rise complete");
                rising = false;
                character.Locomotion.AddMove(new Vector3(0,jumpEndSpeed,0));
                //EndCurrentState();
            }

            if (!rising && character.Locomotion.currentMovement.y <= 0)
            {
                EndCurrentState();
            }
            
        }

        public override void OnExit()
        {
            base.OnExit();
            rising = false;
        }

        private void FixedUpdate()
        {
            if(rising) character.Locomotion.AddExtraMove(new Vector3(0,jumpSpeed,0));
        }
    }
}
