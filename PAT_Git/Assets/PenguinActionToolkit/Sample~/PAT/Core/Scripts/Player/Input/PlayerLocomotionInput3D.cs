using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace PAT
{
    public class PlayerLocomotionInput3D : MonoBehaviour, PlayerLocomotionInputBase
    {
        private Character _character;
        [SerializeField] private AimAbility aimer;
        [SerializeField] private InputActionReference _actionRef;
        [SerializeField] private Transform planeRef;
        [SerializeField] public InputAxis currentAxis = null;

        protected InputAction _action;
        public Vector3 CalibrateInputDirection(Camera playerCam)
        {
            //Get Direction Modified by Camera//
            Transform camTransform = playerCam.transform;

            Vector3 currentDirection = _action.ReadValue<Vector2>().x * camTransform.right;
            currentDirection += _action.ReadValue<Vector2>().y * camTransform.forward;

            if (currentAxis != null)
            {
                currentDirection = Vector3.ProjectOnPlane(currentDirection, currentAxis.up).normalized;
            }
            
            return currentDirection * _action.ReadValue<Vector2>().magnitude;
        }
        
        void Initialize(Player player)
        {
            _actionRef.action.actionMap.Enable();
            _action = player.ResolveForPlayer(_actionRef);
            if(planeRef)currentAxis = new InputAxis(planeRef);
            else currentAxis = new InputAxis();
        }

        public void ProcessLocomotionInputs(Player player)
        {
            if(_action == null){ Initialize(player);}
            
            //See if we need to update aimer, because aimer is cached 
            if (player.character != _character)
            {
                _character = player.character;
                aimer = player.character.GetComponent<AimAbility>();
            }
            
            //Handle Input Direction
            player.character.Locomotion.SetCurrentMoveDirection(CalibrateInputDirection(player.playerCam));
            
            //Handle Rotation
            if(aimer != null) AimerDrivenRotation(player.playerCam, player.character);
            else NoAimerRotation(player.playerCam, player.character);
        }
        
        void AimerDrivenRotation(Camera playerCam, Character playerCharacter)
        {
            var newDirection = CalibrateInputDirection(playerCam);
            newDirection.y = 0;
            if(newDirection != Vector3.zero) aimer.SetCharacterRotation(newDirection);
        }

        void NoAimerRotation(Camera playerCam, Character playerCharacter)
        {
            var newDirection = CalibrateInputDirection(playerCam);
            newDirection.y = 0;
            if(newDirection != Vector3.zero) playerCharacter.Locomotion.SetCurrentRotateDirection(newDirection);
        }
        
    }
}
