using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    /// <summary>
    /// Send additional main tag & input tag to a character base on locomotion behavior 
    /// </summary>
    public class LocomotionInfoAbility: MonoBehaviour
    {
        public Character owner;
        public GamePlayTag startMovingTag = GamePlayTag.forward;
        public GamePlayTag stopMovingTag = GamePlayTag.backward;
        public GamePlayTag startRisingTag = GamePlayTag.up;
        public GamePlayTag startFallingTag = GamePlayTag.CanFall;
        public GamePlayTag landingTag = GamePlayTag.down;
        public GamePlayTag turningTag = GamePlayTag.right;

        public float startMoveSpeed = 0.1f;
        public float stopMoveSpeed = 5f;
        public float turnAroundSpeed = 5f;
        public float turnAroundAngle = 90f;
        
        public Vector3 _lastMovement;
        public Vector3 _lastDirection;
        public bool _lastOnGround;
        
        private void Awake()
        {
            if(!owner) owner = GetComponent<Character>();
            if(!owner) {Debug.LogWarning("No Character assigned for this ability");}
        }

        private void Update()
        {
            if(!owner) return;

            owner.tagContainer.inputTags.Remove(startMovingTag);
            owner.tagContainer.inputTags.Remove(stopMovingTag);
            owner.tagContainer.inputTags.Remove(startRisingTag);
            owner.tagContainer.inputTags.Remove(landingTag);
            owner.tagContainer.inputTags.Remove(turningTag);
            owner.tagContainer.inputTags.Remove(startFallingTag);

            Vector3 _currentMovement = owner.Locomotion.currentMovement;
            Vector3 _currentDirection = owner.Locomotion.currentMoveDirection;
            bool _onGround = owner.Locomotion.onGround;

            if (_onGround
                && _lastMovement.magnitude < startMoveSpeed
                && _currentMovement.magnitude >= startMoveSpeed)
            {
                Debug.Log("start moving");
                owner.tagContainer.inputTags.Add(startMovingTag);
            }

            if (_onGround
                && _lastMovement.magnitude >= stopMoveSpeed
                && _lastDirection.magnitude > 0.1f
                 && _currentDirection.magnitude <= 0.1f)
            {
                Debug.Log("stop moving");
                owner.tagContainer.inputTags.Add(stopMovingTag);
            }

            if (_onGround
                && _lastMovement.magnitude >= turnAroundSpeed
                && Vector3.Angle(_lastMovement.normalized, _currentDirection) > 90f)
            {
                Debug.Log("start turning");
                owner.tagContainer.inputTags.Add(turningTag);
            }

            if (!_lastOnGround && _onGround)
            {
                Debug.Log("landed");
                owner.tagContainer.inputTags.Add(landingTag);
            }

            if (_lastMovement.y >= 0 && _currentMovement.y < 0)
            {
                Debug.Log("start falling");
                owner.tagContainer.inputTags.Add(startFallingTag);
            }

            if (_lastMovement.y <= 0 && _currentMovement.y > 0)
            {
                Debug.Log("start rising");
                owner.tagContainer.inputTags.Add(startRisingTag);
            }
            
            _lastMovement = _currentMovement;
            _lastDirection = _currentDirection;
            _lastOnGround = _onGround;
        }
    }
}