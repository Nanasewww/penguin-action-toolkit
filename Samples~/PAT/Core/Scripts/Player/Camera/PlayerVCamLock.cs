using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

namespace PAT
{
    public class PlayerVCamLock: MonoBehaviour
    {
        [SerializeField] protected int priorityWhenActivate = 5;
        [SerializeField] protected Player player;
        [SerializeField] protected CinemachineVirtualCamera vcam;
        [Tooltip("A button input is enough")]
        public InputActionReference LockInputAction;
        private InputAction _lockAction;
        [Tooltip("Need a float value")]
        public InputActionReference SwapInputAction;
        private InputAction _swapAction;
        public AimTarget currentTarget;
        
        const float disRatioValue = 0.05f;
        
        protected bool activated = false;
        protected AimAbility characterAim;
        
        public bool IsActive(){return activated;}
        
        private void Reset()
        {
            player = GetComponentInParent<Player>();
        }

        private void Awake()
        {
            if(!player) player = GetComponentInParent<Player>();
            if (player) player.OnCharacterChange += AssignTarget;
            else Debug.LogWarning("No player assigned for player cam");
            
            if (!vcam) vcam = GetComponent<CinemachineVirtualCamera>();
            
            if(LockInputAction) LockInputAction.action.Enable();
            if(SwapInputAction) SwapInputAction.action.Enable();
        }
        
        /// <summary>
        /// Helps Keep Track of Player
        /// </summary>
        /// <param name="character"></param>
        public void AssignTarget(Character character)
        {
            vcam.Follow = character.characterModel.transform;
            characterAim = character.GetComponentInChildren<AimAbility>();
        }

        private void Update()
        {
            if (_lockAction == null || _swapAction == null)
            {
                _lockAction = player.ResolveForPlayer(LockInputAction);
                _swapAction = player.ResolveForPlayer(SwapInputAction);
                return;
            }
            
            if(_lockAction.WasPressedThisFrame()) SwitchCameraMode();
            if(_swapAction.WasPressedThisFrame()) SwapTarget(SwapInputAction.action.ReadValue<float>());

            if (currentTarget)
            {
                vcam.LookAt = currentTarget.transform;
                if (activated && characterAim) characterAim.SetHardLockTarget(currentTarget);
            }
        }
        
        
        void OnTargetMiss()
        {
            if(currentTarget) currentTarget.onRemoveFromList -= OnTargetMiss;
            currentTarget = null;
            GetMostCenteredLockAble();

            if (currentTarget == null) SwitchCameraMode();
            else currentTarget.onRemoveFromList += OnTargetMiss;
        }
        
        public void SwitchCameraMode()
        {
            if (!activated && GetMostCenteredLockAble())
            {
                activated = true;
                vcam.Priority = priorityWhenActivate;
                currentTarget.onRemoveFromList += OnTargetMiss;
            }

            else if (activated)
            {
                if (characterAim) characterAim.SetHardLockTarget(null);
                activated = false;
                vcam.Priority = 0;
            }
        }
        
        public AimTarget GetMostCenteredLockAble()
        {
            if (AimTarget.LockAbleList == null || AimTarget.LockAbleList.Count <= 0)
            {
                return null;
            }

            Camera cam = player.playerCam;

            if (!cam) {Debug.LogWarning("No Main Cam"); return null; }

            foreach (var lockAble in AimTarget.LockAbleList)
            {
                if(lockAble.GetTeam() == player.character.team) continue;
                if(lockAble.CameraDistance(cam) > 40) continue;
                if (currentTarget == null) currentTarget = lockAble;
                
                Vector3 currentScreenDif = new Vector3(cam.pixelWidth / 2f, cam.pixelHeight / 2f, 0) - currentTarget.CameraPosition(cam);
                Vector3 newScreenDif = new Vector3(cam.pixelWidth / 2f, cam.pixelHeight / 2f, 0) - lockAble.CameraPosition(cam);

                Vector3 currentDistance = cam.transform.position - currentTarget.transform.position;
                Vector3 newDistance = cam.transform.position - lockAble.transform.position;

                if ((newDistance.magnitude * disRatioValue + newScreenDif.magnitude / cam.pixelWidth)
                    < (currentDistance.magnitude * disRatioValue + currentScreenDif.magnitude / cam.pixelWidth))
                {
                    currentTarget = lockAble;
                }

            }
            
            return currentTarget;
        }

        public void SwapTarget(float direction)
        {

            if (AimTarget.LockAbleList == null) { return; }
            if (!currentTarget) { return; }

            AimTarget result = null;
            float resultDifX = 0;
            
            Camera cam = Camera.main;

            if (!cam) {Debug.LogWarning("No Main Cam"); return; }

            foreach (var lockAble in AimTarget.LockAbleList)
            {
                if(lockAble.GetTeam() == player.character.team) continue;
                
                float difX = lockAble.CameraPosition(cam).x - currentTarget.CameraPosition(cam).x;

                //Skip self
                if (lockAble == currentTarget) continue;
                //Same direction Check
                if (difX * direction < 0) continue;

                //If closer, or the first valid one
                if (Mathf.Abs(difX) < Mathf.Abs(resultDifX) || resultDifX == 0)
                {
                    result = lockAble;
                    resultDifX = difX;
                }
            }

            if (result) { currentTarget = result; }
        }
    }
}