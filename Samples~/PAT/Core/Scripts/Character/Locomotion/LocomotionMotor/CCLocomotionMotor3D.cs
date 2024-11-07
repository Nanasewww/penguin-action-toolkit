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
        [SerializeField] protected float stableOnGroundDistance = 0.3f;
        [SerializeField] [Tooltip("Direction for Raycast")] protected Vector3 gravityDirection = new Vector3(0, -1, 0);
        
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
            controller.Move((locomotion.currentMovement + locomotion.extraMovement) * Time.fixedDeltaTime);
            
            //Snap to ground section

            
            RaycastHit hit; 
            Vector3 rootPosition = controller.transform.position + controller.center - new Vector3(0, controller.height * 0.5f, 0) ;
            Physics.Raycast(rootPosition, gravityDirection,  out hit, stableOnGroundDistance, groundLayer);
            
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
                locomotion.currentAttribute.rotationSpeed * Time.fixedDeltaTime);
            controller.enabled = true;
        }

        public bool CheckIfGrounded(CharacterLocomotionBase locomotion)
        {
            Vector3 rootPosition = controller.transform.position + controller.center - new Vector3(0, controller.height * 0.5f, 0) ;
            if (Physics.Raycast(rootPosition, gravityDirection, stableOnGroundDistance, groundLayer)) return true;
            if (Physics.Raycast(rootPosition + new Vector3(controller.radius, 0, 0), gravityDirection, stableOnGroundDistance, groundLayer)) return true;
            if (Physics.Raycast(rootPosition + new Vector3(-controller.radius, 0, 0), gravityDirection, stableOnGroundDistance, groundLayer)) return true;
            if (Physics.Raycast(rootPosition + new Vector3(0 ,0 , controller.radius), gravityDirection, stableOnGroundDistance, groundLayer)) return true;
            if (Physics.Raycast(rootPosition + new Vector3(0 ,0 , -controller.radius), gravityDirection, stableOnGroundDistance, groundLayer)) return true;

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