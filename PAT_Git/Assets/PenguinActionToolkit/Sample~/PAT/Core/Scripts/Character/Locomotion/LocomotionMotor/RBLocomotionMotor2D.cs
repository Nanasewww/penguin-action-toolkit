using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using UnityEditor;

namespace PAT
{
    [Serializable]
    public class AxisDirection
    {
        public Vector3 forward = new(1, 0, 0);
        public readonly Vector3 up = new(0, 1, 0);
        public readonly Vector3 right;

        public AxisDirection()
        {
            forward = this.forward.normalized;
            right = Vector3.Cross(this.forward, this.up).normalized;
        }
    }
    public class RBLocomotionMotor2D : MonoBehaviour, CharacterLocomotionMotorBase, ICharacterController
    {
        [Header("Special Attributes")] 
        [SerializeField] protected KinematicCharacterMotor _motor;
        
        [SerializeField] public AxisDirection baseAxis;
        [SerializeField] public AxisDirection currentAxis;
        [SerializeField] public float characterAngle = 30.0f;
        
        [SerializeField] private Vector3 finalMovement;
        private Vector3 lastDirection;

        private Character character;

        private void Awake()
        {
            currentAxis = baseAxis;
            if (!_motor) _motor = GetComponent<KinematicCharacterMotor>();
            if (!_motor) _motor = GetComponentInParent<KinematicCharacterMotor>();
            if (_motor) _motor.CharacterController = this;
        }

        private void Start()
        {
            character = GetComponent<Character>();
            if (character == null) character = GetComponentInParent<Character>();
        }

        public void ApplyMovement(CharacterLocomotionBase locomotion)//Vector3 currentMovement, Vector3 extraMovement)
        {
            Vector3 currentMovement = locomotion.currentMovement;
            Vector3 extraMovement = locomotion.extraMovement;
            
            if(Vector3.Dot(extraMovement, currentAxis.up) != 0) _motor.ForceUnground();
            if(Vector3.Dot(currentMovement, currentAxis.up) > 0) _motor.ForceUnground();
            finalMovement = Vector3.Dot(currentMovement + extraMovement, currentAxis.forward) * currentAxis.forward;
            finalMovement += Vector3.Dot(currentMovement + extraMovement, currentAxis.up) * currentAxis.up;
        }
        
        //todo: check if this is working
        public void ApplyRotation(CharacterLocomotionBase locomotion)//Quaternion finalRotation, GameObject model)
        {
            Vector3 currentDirection = locomotion.currentRotateDirection;
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
                locomotion.characterTransform.rotation = Quaternion.RotateTowards(locomotion.transform.rotation,
                                Quaternion.LookRotation(newDirection), 90.0f);
                //locomotion.CurrentData.rotationSpeed * Time.fixedDeltaTime);
            }
            lastDirection = newDirection;
            
            
            // Mirror character model in 2D game
            GameObject model = locomotion.character.characterModel;
            Vector3 preScale = model.transform.localScale;
            
            preScale.x = Mathf.Abs(preScale.x);
            model.transform.localScale = Vector3.Scale(preScale, new Vector3(directionSign,1,1));
            model.transform.localEulerAngles = directionSign * characterAngle * currentAxis.up;
        }
        
        public bool CheckIfGrounded(CharacterLocomotionBase locomotion)
        {
            return _motor.GroundingStatus.IsStableOnGround;
        }
        
        public void MoveToPosition(CharacterLocomotionBase locomotion, Vector3 destPosition)
        {
            locomotion.characterTransform.position = destPosition;
        }

        public void InstantRotate(CharacterLocomotionBase locomotion, float angle)
        {
            throw new NotImplementedException();
        }

        public void RotateToward(CharacterLocomotionBase locomotion, Vector3 dir, float deltaTime)
        {
            throw new NotImplementedException();
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            currentRotation = character.transform.rotation;
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            currentVelocity = finalMovement;
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
            
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
            
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
            
        }
    }
}
