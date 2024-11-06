using System;
using UnityEngine;

namespace PAT
{
    public class LocomotionAnimController3D: MonoBehaviour, LocomotionAnimControllerBase
    {
        [SerializeField] private Animator _animator;

        private int aniIDXSpeed;
        private int aniIDZSpeed;
        private int aniIDLocomotionSpeed;
        private int aniGround;

        public void Initialize(CharacterLocomotionBase locomotion)
        {
            if (_animator == null)
            {
                _animator = locomotion.character.modelHandler.GetAnimator();
            }
            if (_animator)
            {
                aniIDXSpeed = Animator.StringToHash("XSpeed");
                aniIDZSpeed = Animator.StringToHash("ZSpeed");
                aniIDLocomotionSpeed = Animator.StringToHash("LocomotionSpeed");
                aniGround = Animator.StringToHash("OnGround");
            }
        }

        public void MovementAnimation(CharacterLocomotionBase locomotion)
        {
            //This shall be related to transform
            Vector3 planeMove = locomotion.characterTransform.InverseTransformDirection(locomotion.currentMovement);
            planeMove.y = 0;
            
            //todo: shall this be the only solution? Is it performing well?
            _animator.SetFloat(aniIDXSpeed, planeMove.z/ locomotion.currentAttribute.MoveSpeed);
            _animator.SetFloat(aniIDZSpeed, planeMove.x/ locomotion.currentAttribute.MoveSpeed);
            
            _animator.SetFloat(aniIDLocomotionSpeed, locomotion.currentAttribute.locomotionAnimationSpeed);
            _animator.SetBool(aniGround, locomotion.onGround);
        }
    }
}