using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    public class CCLocomotionMotor3D: MonoBehaviour, CharacterLocomotionMotorBase
    {
        [Header("Special Attributes")]
        [SerializeField] protected CharacterController controller;
        [SerializeField] protected LayerMask groundLayer;
        [SerializeField] protected float snapToGroundDistance = 0.5f;
        [SerializeField] protected float additionalGroundCheckDistance = 0.2f;
        [SerializeField] [Tooltip("Direction for Raycast")] protected Vector3 gravityDirection = new Vector3(0, -1, 0);
        
        public Vector3 rootPosition { get{ return controller.transform.position + controller.center - new Vector3(0, controller.height * 0.5f, 0) ;}}
        private void Awake()
        {
            if (controller == null) controller = GetComponent<CharacterController>();
            if (controller == null) controller = gameObject.AddComponent<CharacterController>();
        }

        private void Reset()
        { 
            groundLayer = LayerMask.GetMask("Default", "Ground");
        }

        public void ApplyMovement(CharacterLocomotionBase locomotion)
        {
            controller.Move((locomotion.currentMovement + locomotion.extraMovement) * locomotion.FixedDeltaTime());
            
            //Snap to ground section
            
            RaycastHit hit; 
            Physics.Raycast(rootPosition, gravityDirection,  out hit, snapToGroundDistance, groundLayer);

            Vector3 dif = hit.point - rootPosition;

            if (hit.collider != null && (locomotion.currentMovement + locomotion.extraMovement).y <= 0)
            {
                controller.Move(dif);
            }
            
        }

        public void ApplyRotation(CharacterLocomotionBase locomotion)
        {
            controller.enabled = false;
            locomotion.characterTransform.rotation = Quaternion.RotateTowards(controller.transform.rotation, Quaternion.LookRotation(locomotion.currentRotateDirection), 
                locomotion.currentAttribute.rotationSpeed * locomotion.FixedDeltaTime());
            controller.enabled = true;
        }

        public bool CheckIfGrounded(CharacterLocomotionBase locomotion)
        {
            if (controller.isGrounded) return true;
            if (Physics.Raycast(rootPosition, gravityDirection, additionalGroundCheckDistance, groundLayer)) return true;    

            if (Physics.Raycast(rootPosition + new Vector3(controller.radius, 0, 0), gravityDirection, additionalGroundCheckDistance, groundLayer)) return true;
            if (Physics.Raycast(rootPosition + new Vector3(-controller.radius, 0, 0), gravityDirection, additionalGroundCheckDistance, groundLayer)) return true;
            if (Physics.Raycast(rootPosition + new Vector3(0 ,0 , controller.radius), gravityDirection, additionalGroundCheckDistance, groundLayer)) return true;
            if (Physics.Raycast(rootPosition + new Vector3(0 ,0 , -controller.radius), gravityDirection, additionalGroundCheckDistance, groundLayer)) return true;
            

            return false;
        }
        
        
        public void MoveToPosition(CharacterLocomotionBase locomotion, Vector3 destPosition)
        {
            controller.enabled = false;
            locomotion.characterTransform.position = destPosition;
            controller.enabled = true;
        }

        public void InstantRotate(CharacterLocomotionBase locomotion, float angle)
        {
            controller.enabled = false;
            locomotion.characterTransform.Rotate(new Vector3(0, angle, 0));
            controller.enabled = true;
        }
    }
}