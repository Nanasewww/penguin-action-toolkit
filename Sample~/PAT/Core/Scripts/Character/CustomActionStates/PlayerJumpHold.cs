using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

namespace PAT
{
    public class PlayerJumpHold : ActionState
    {
        [SerializeField] protected float jumpHeightMax;
        [SerializeField] protected float jumpHeightMin;
        [SerializeField] protected float jumpSpeed;
        [SerializeField] protected float jumpEndSpeed;
        [SerializeField] protected float coyoteTime = 0.1f;

        private float jumpTimeMin, jumpTimeMax;
        private bool rising = false;
        private bool pressedThisFrame = false;
        private bool pressedLastFrame = false;
        
        private void Reset()
        {
            _mainTags = new List<GamePlayTag> { GamePlayTag.Jumping };
            jumpHeightMax = 3;
            jumpHeightMin = 1;
            jumpSpeed = 20;
            jumpEndSpeed = 5;
        }
        /*public override bool Validation()
        {
            if (!controller.Locomotion.OnGround && CheckUngrounded()) return false;

            return base.Validation();
        }*/

        public override bool Triggered()
        {
            pressedThisFrame = character.tagContainer.inputTags.Contains(_inputTag);
            if (_inputTag != GamePlayTag.None && pressedThisFrame && !pressedLastFrame)
            {
                pressedLastFrame = true;
                return true;
            }
            pressedLastFrame = pressedThisFrame;
            
            return false;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            jumpTimeMin = jumpHeightMin / jumpSpeed;
            jumpTimeMax = jumpHeightMax / jumpSpeed;
            
            rising = true;
        }

        public override void OnTick(float deltaTime)
        {
            base.OnTick(deltaTime);

            pressedThisFrame = character.tagContainer.inputTags.Contains(_inputTag);
            if (rising && (timeInState > jumpTimeMax || 
                           (!pressedThisFrame && timeInState > jumpTimeMin)))
            {
                rising = false;
                character.Locomotion.AddMove(new Vector3(0,jumpEndSpeed,0));
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

        public void Update()
        {
            //if (!rising) CheckUngrounded();
        }

        /*private bool CheckUngrounded()
        {
            if (controller.Locomotion.OnGround) { ungroundedTime = 0; }
            else { ungroundedTime += Time.deltaTime; }

            return ungroundedTime > coyoteTime;
        }*/
    }
}
