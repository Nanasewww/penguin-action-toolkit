using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

namespace PAT
{
    public class PlayerVCamFree: MonoBehaviour, AxisState.IInputAxisProvider
    {
        [SerializeField] protected Player player;
        [SerializeField] protected CinemachineVirtualCamera vcam;
        [Tooltip("Vector2 action for XY Rotation")]
        public InputActionReference XYInput;
        public bool recenter = true;
        
        protected CinemachinePOV pov;
        protected InputAction xyAction;

        private void Reset()
        {
            player = GetComponent<Player>();
        }

        private void Awake()
        {
            if (!player) player = GetComponentInParent<Player>();
            if (player) player.OnCharacterChange += AssignTarget;
            else Debug.LogWarning("No player assigned for player cam");
            
            if (!vcam) vcam = GetComponent<CinemachineVirtualCamera>();

        }

        private void Start()
        {
            if (vcam)
            {
                pov = vcam.GetCinemachineComponent<CinemachinePOV>();
                pov.m_VerticalAxis.SetInputAxisProvider(1,this);
                pov.m_HorizontalAxis.SetInputAxisProvider(0,this);
            }
            else Debug.LogWarning("No virtual camera assigned");

            xyAction = player.ResolveForPlayer(XYInput);
        }

        /// <summary>
        /// Helps Keep Track of Player
        /// </summary>
        /// <param name="character"></param>
        public void AssignTarget(Character character)
        {
            vcam.Follow = character.characterModel.transform;
            vcam.LookAt = character.characterModel.transform;
        }
        
        private void FixedUpdate()
        {
            if (pov == null) return;
            pov.m_HorizontalRecentering.m_enabled = recenter;
            pov.m_VerticalRecentering.m_enabled = recenter;
            
        }

        public float GetAxisValue(int axis)
        {
            if (xyAction != null)
            {
                switch (axis)
                {
                    case 0: return xyAction .ReadValue<Vector2>().x;
                    case 1: return xyAction .ReadValue<Vector2>().y;
                    case 2: return xyAction .ReadValue<float>();
                } 
            }
            xyAction = player.ResolveForPlayer(XYInput);
            return 0; 
        }
    }
}