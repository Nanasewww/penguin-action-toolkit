using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

namespace PAT
{
    public class RBLocomotionMotor3D : MonoBehaviour, CharacterLocomotionMotorBase, ICharacterController
    {
        [Header("Special Attributes")] 
        [SerializeField] protected KinematicCharacterMotor _motor;
        [SerializeField] private Vector3 finalMovement;

        private Character character;

        private void Awake()
        {
            if (!_motor) _motor = GetComponent<KinematicCharacterMotor>();
            if (!_motor) _motor = GetComponentInParent<KinematicCharacterMotor>();
            if (_motor) _motor.CharacterController = this;
        }

        private void Start()
        {
            character = GetComponent<Character>();
            if (character == null) character = GetComponentInParent<Character>();
        }

        public void ApplyMovement(CharacterLocomotionBase locomotion)
        {
            if(Vector3.Dot(locomotion.extraMovement, Vector3.up) != 0) _motor.ForceUnground();
            if(Vector3.Dot(locomotion.currentMovement, Vector3.up) > 0) _motor.ForceUnground();
            finalMovement = locomotion.currentMovement + locomotion.extraMovement;
        }
        
        public void ApplyRotation(CharacterLocomotionBase locomotion)
        {
            locomotion.characterTransform.rotation = Quaternion.RotateTowards(locomotion.characterTransform.rotation, Quaternion.LookRotation(locomotion.currentRotateDirection), 
                locomotion.currentAttribute.rotationSpeed * Time.fixedDeltaTime);
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
            locomotion.characterTransform.Rotate(new Vector3(0, angle, 0));
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
