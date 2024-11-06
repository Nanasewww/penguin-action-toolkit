using System;
using UnityEngine;

namespace PAT
{
    public class CCLocomotionMotor2D: MonoBehaviour, CharacterLocomotionMotorBase
    {
        [Header("Special Attributes")] 
        [SerializeField] protected CharacterController controller;
        [SerializeField] protected AxisDirection baseAxis;
        [SerializeField] protected AxisDirection currentAxis;
        
        private Vector3 lastDirection;
        
        private void Awake()
        {
            currentAxis = baseAxis;
        }

        public void ApplyMovement(CharacterLocomotionBase locomotion)
        {
            Vector3 currentMovement = locomotion.currentMovement;
            Vector3 extraMovement = locomotion.extraMovement;
            
            Vector3 finalMovement = Vector3.Dot(currentMovement + extraMovement, currentAxis.forward) * currentAxis.forward;
            finalMovement += Vector3.Dot(currentMovement + extraMovement, currentAxis.up) * currentAxis.up;
            controller.Move(finalMovement * Time.fixedDeltaTime);
        }

        //check if this is working
        public void ApplyRotation(CharacterLocomotionBase locomotion)
        {
            Vector3 currentDirection = locomotion.currentMoveDirection;
            Vector3 newDirection;
            float directionSign = Mathf.Sign(Vector3.Dot(locomotion.characterTransform.forward, currentAxis.forward));
            
            if (currentDirection == Vector3.zero)
            {
                if (Vector3.Dot(locomotion.characterTransform.forward, currentAxis.right) == 0) return;
                newDirection = lastDirection;
            }
            else { newDirection = currentDirection; }
            
            // Rotate character with input direction
            newDirection.y = 0;
            if (newDirection != Vector3.zero)
            {
                locomotion.characterTransform.rotation = Quaternion.RotateTowards(locomotion.characterTransform.rotation,
                    Quaternion.LookRotation(newDirection), 90.0f);
            }
            lastDirection = newDirection;
        }
        
        public void InstantRotate(CharacterLocomotionBase locomotion, float angle)
        {
            throw new NotImplementedException();
        }

        public void RotateToward(CharacterLocomotionBase locomotion, Vector3 dir, float deltaTime)
        {
            throw new NotImplementedException();
        }

        public bool CheckIfGrounded(CharacterLocomotionBase locomotion)
        {
            return false;
        }
        
        public void MoveToPosition(CharacterLocomotionBase locomotion, Vector3 destPosition)
        {
            GetComponent<CharacterController>().enabled = false;
            locomotion.characterTransform.position = destPosition;
            GetComponent<CharacterController>().enabled = true;
        }

        
    }
}