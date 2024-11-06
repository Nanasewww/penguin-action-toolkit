using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PAT
{
    public class NavAgentLocomotionMotor3D : MonoBehaviour, CharacterLocomotionMotorBase
    {
        [SerializeField] protected NavMeshAgent agent;
        
        public void Awake()
        {
            if (!agent) { agent = GetComponent<NavMeshAgent>(); }
        }

        public void ApplyMovement(CharacterLocomotionBase locomotion)
        {
            //Apply The Movement
            agent.Move((locomotion.currentMovement + locomotion.extraMovement) * Time.fixedDeltaTime);
        }

        public void ApplyRotation(CharacterLocomotionBase locomotion)
        {
            locomotion.characterTransform.rotation = Quaternion.RotateTowards(locomotion.characterTransform.rotation, Quaternion.LookRotation(locomotion.currentRotateDirection), 
                locomotion.currentAttribute.rotationSpeed * Time.fixedDeltaTime);
        }

        public bool CheckIfGrounded(CharacterLocomotionBase locomotion)
        {
            return true;
        }
        
        public void MoveToPosition(CharacterLocomotionBase locomotion, Vector3 destPosition)
        {
            return;
        }
        
        public void InstantRotate(CharacterLocomotionBase locomotion, float angle)
        {
            locomotion.characterTransform.Rotate(new Vector3(0, angle, 0));
        }
    }
}
