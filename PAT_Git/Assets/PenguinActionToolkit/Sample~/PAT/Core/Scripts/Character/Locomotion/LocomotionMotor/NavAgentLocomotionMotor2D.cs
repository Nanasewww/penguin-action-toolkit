using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace PAT
{
    //todo: I think this shouldn't exist 
    public class NavAgentLocomotionMotor2D : MonoBehaviour, CharacterLocomotionMotorBase
    {
        [SerializeField] protected NavMeshAgent agent;
        
        [Header("Special Attributes")] 
        [SerializeField] public AxisDirection baseAxis;
        [SerializeField] public AxisDirection currentAxis;
        
        public void Awake()
        {
            if (!agent) { agent = GetComponent<NavMeshAgent>(); }
            //baseAxis.Normalize();
            currentAxis = baseAxis;
        }

        public void ApplyMovement(CharacterLocomotionBase locomotion)
        {
            Vector3 movement = locomotion.currentMovement + locomotion.extraMovement;
            //Apply The Movement
            Vector3 finalMovement = Vector3.Dot(movement, currentAxis.forward) * currentAxis.forward;
            finalMovement += Vector3.Dot(movement, currentAxis.up) * currentAxis.up;
            agent.Move(finalMovement * Time.fixedDeltaTime);
        }

        //todo: this is 2d, shall this really be like that?
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

        //todo: this is 2d, shall this really be like that?
        public void InstantRotate(CharacterLocomotionBase locomotion, float angle)
        {
            locomotion.characterTransform.Rotate(new Vector3(0, angle, 0));
        }
        
    }
}
