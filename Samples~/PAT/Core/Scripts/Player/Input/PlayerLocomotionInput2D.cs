using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace PAT
{
    [Serializable]
    public class InputAxis
    {
        public Vector3 forward = new(0, 0, 1);
        public Vector3 right = new(1, 0, 0);
        public Vector3 up = new(0, 1, 0);

        public InputAxis()
        {
            
        }

        public InputAxis(Transform t)
        {
            forward = t.forward;
            right = t.right;
            up = t.up;
        }
    }
    
    public class PlayerLocomotionInput2D : MonoBehaviour, PlayerLocomotionInputBase
    {
        [SerializeField] public InputAxis baseAxis;
        [SerializeField] public InputAxis currentAxis;
        [SerializeField] private InputActionReference _actionRef;
        
        protected InputAction _action;
        private void Awake()
        {
            currentAxis = baseAxis;
        }
        
        void Initialize(Player player)
        {
            _actionRef.action.actionMap.Enable();
            _action = player.ResolveForPlayer(_actionRef);
        }

        public Vector3 CalibrateInputDirection()
        {
            Vector3 currentDirection = _action.ReadValue<Vector2>().x * currentAxis.forward;
            return currentDirection;
        }

        public Vector3 RawInputDirection()
        {
            Vector3 rawDirection = _action.ReadValue<Vector2>().x * currentAxis.forward;
            rawDirection += _action.ReadValue<Vector2>().y * currentAxis.up;
            return rawDirection;
        }

        public void ProcessLocomotionInputs(Player player)
        {
            if(_action == null){ Initialize(player);}
            
            //Handle Input Direction
            player.character.Locomotion.SetCurrentMoveDirection(CalibrateInputDirection());
            player.character.Locomotion.SetCurrentRotateDirection(CalibrateInputDirection());
            player.character.Locomotion.SetCurrentInputDirection(RawInputDirection());
        }
        
    }
}
