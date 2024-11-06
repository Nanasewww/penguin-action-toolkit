using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    /// <summary>
    /// This is the example of an ability
    /// It provides some complex functionality that applies to character, that involves interaction between each character or Player
    /// And will be used by State Mod, or interact with player control, AI somehow
    /// </summary>
    public class AimAbility: MonoBehaviour
    {
        [SerializeField]private Character character;
        [SerializeField]private CharacterLocomotionBase characterLocomotion;
        
        [SerializeField]private AimTarget  _softLockTarget;
        [SerializeField]private AimTarget _hardLockTarget;

        private void Awake()
        {
            if(character == null) character = GetComponent<Character>();
        }

        private void Start()
        {
            if(characterLocomotion == null) characterLocomotion = character.Locomotion;
        }

        /// <summary>
        /// This is ask the aim try to rotate the character toward a direction
        /// However, this only works in the aim is not locking already
        /// </summary>
        /// <param name="dir"></param>
        public void SetCharacterRotation(Vector3 dir)
        {
            if(_softLockTarget) return;
            if(_hardLockTarget) return; 
            
            characterLocomotion.SetCurrentRotateDirection(dir);
        }

        public void SetHardLockTarget(AimTarget target)
        {
            _hardLockTarget = target;
        }

        public Transform GetCurrentLockTarget()
        {
            if (_hardLockTarget) return _hardLockTarget.transform;
            if (_softLockTarget) return _softLockTarget.transform;
            return null;
        }
        
        public void AttemptSoftLock(float distanceRange, float angleRange)
        {
            //The default application is closet target
            float minDis = 0;
            AimTarget target = null;
            foreach (AimTarget c in AimTarget.LockAbleList)
            {
                //Skip teammates
                if(c.GetTeam() == character.team) continue;
                
                float dis = Vector3.Distance(c.transform.position, character.transform.position);
                Vector3 dir = c.transform.position - character.transform.position;
                float angle = Vector3.Angle(character.transform.forward, dir);
                
                if(dis > distanceRange) continue;
                if(angle > angleRange/2) continue;
                
                if (dis < minDis || target == null)
                {
                    target = c;
                    minDis = dis;
                }
            }
            if(target)_softLockTarget = target;
        }
        
        public void EndSoftLock()
        {
            _softLockTarget = null;
            //Clear the rotation to avoid unexpected behavior
            //characterLocomotion.SetCurrentRotateDirection(Vector3.zero);
        }

        private void Update()
        {
            Transform lockTarget = null;
            //Hard lock have higher priority
            if(_softLockTarget) lockTarget = _softLockTarget.transform;
            if(_hardLockTarget) lockTarget = _hardLockTarget.transform;
            
            //Rotate the character toward lock target if there is one
            if(lockTarget == null) return;
            Vector3 direction = lockTarget.position - transform.position;
            direction = direction.normalized;
            direction.y = 0;
            
            characterLocomotion.SetCurrentRotateDirection(direction);
        }
    }
}