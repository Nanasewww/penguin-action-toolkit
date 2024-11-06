using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PAT
{
    /// <summary>
    /// This is an example of combined action states
    /// </summary>
    public class PlayerDashDirectional : ActionState
    {
        protected AnimationMontageMod _animationMontageMf;
        protected AnimationMontageMod _animationMontageMf2;
        protected LocomotionMod _locoMf1;
        protected LocomotionMod _locoMf2;
        protected GrantTagMod GrantTagMod;


        private void Awake()
        {
            _animationMontageMf = gameObject.AddComponent<AnimationMontageMod>();
            _animationMontageMf2 = gameObject.AddComponent<AnimationMontageMod>();
            _locoMf1 = gameObject.AddComponent<LocomotionMod>();
            _locoMf2 = gameObject.AddComponent<LocomotionMod>();
            GrantTagMod = gameObject.AddComponent<GrantTagMod>();


            _animationMontageMf.info.layer = 0;
            _animationMontageMf.info.fadeOutTime = 0.25f;
            _animationMontageMf.mode = ModifierMode.ByTimeInState;
            
            _animationMontageMf2.info.layer = 2;
            _animationMontageMf2.info.fadeOutTime = 0.25f;
            _animationMontageMf2.mode = ModifierMode.ByTimeInState;
            
            //===First Period Locomotion===//
            _locoMf1.mode = ModifierMode.ByTimeInState;
            _locoMf1.beginTime = 0; _locoMf1.endTime = -1;
            _locoMf1.locomotionData.rootMotionMutiplier = 0.7f;
            
            //===Second Period Locomotion===///
            _locoMf2.mode = ModifierMode.ByTimeInState;
            _locoMf2.beginTime = 0.23f; _locoMf2.endTime = -1;
            
            _locoMf2.locomotionData.rotationSpeed = 600;
            _locoMf2.locomotionData.rootMotionMutiplier = 0.7f;

            //===Grant Tag===//
            GrantTagMod.mode = ModifierMode.ByTimeInState;
            GrantTagMod.beginTime = 0.35f; GrantTagMod.endTime = -1;

            GrantTagMod.grantTags = new List<GamePlayTag> { GamePlayTag.Idle };
            
            modifiers.Add(_animationMontageMf);
            modifiers.Add(_animationMontageMf2);
            modifiers.Add(_locoMf1);
            modifiers.Add(_locoMf2);
            modifiers.Add(GrantTagMod);
        }

        public override void OnEnter()
        {
            //===Remove The Second Locomotion MF if Freelook===//
            if (character.GetComponent<AimAbility>()?.GetCurrentLockTarget() == null) modifiers.Remove(_locoMf2);
            else if (!modifiers.Contains(_locoMf2)) modifiers.Add(_locoMf2);


            base.OnEnter();

            //===Apply Different Animation & Angle Base On Input Direction===//
            Vector3 dir = character.Locomotion.GetCharacterBasedInputDirection();
            dir.y = 0;
            dir = dir.normalized;
            float angle = Vector3.SignedAngle(new Vector3(0, 0, 1), dir.normalized, Vector3.up);

            //===Case Forward Dash===//
            if (Mathf.Abs(angle) < 30)
            {
                character.Locomotion.InstantRotate(angle);
                _animationMontageMf.info.stateName = "Dash_Forward";
                _animationMontageMf2.info.stateName = "Dash_Forward";
            }

            //===Case Backward Dash===//
            else if (Mathf.Abs(angle) > 135)
            {
                character.Locomotion.InstantRotate(angle - 180);
                _animationMontageMf.info.stateName = "Dash_Backward";
                _animationMontageMf2.info.stateName = "Dash_Backward";
            }

            //===Case Leftward Dash===//
            else if (angle < 0)
            {
                character.Locomotion.InstantRotate(angle + 90);
                _animationMontageMf.info.stateName = "Dash_Leftward";
                _animationMontageMf2.info.stateName = "Dash_Leftward";
            }

            //===Case Rightward Dash===//
            else
            {
                character.Locomotion.InstantRotate(angle - 90);
                _animationMontageMf.info.stateName = "Dash_Rightward"; 
                _animationMontageMf2.info.stateName = "Dash_Rightward";
            }
        }
    }
}
