using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Serialization;

namespace PAT
{
    public class ModelHandler: MonoBehaviour
    {
        [SerializeField] protected Character _owner;
        [SerializeField] protected Animator _animator;
        [SerializeField] protected List<AimTarget> _aimTargets;
        [SerializeField] protected String _montageSpeedParameter = "MontageSpeed";
        
        private Attribute _montageSpeedAttribute;
        
        public event Action<int> OnAnimationEvent;
        private void Awake()
        {
            if(!_owner) _owner = GetComponentInParent<Character>();
            if (_animator == null) _animator = GetComponent<Animator>();
            if (_animator) _animator.updateMode = AnimatorUpdateMode.AnimatePhysics;

            if (_aimTargets == null) _aimTargets = new List<AimTarget>();
            if (_aimTargets.Count == 0)
            {
                foreach (var target in GetComponentsInChildren<AimTarget>())
                {
                    _aimTargets.Add(target);
                }
            }
            if (_aimTargets.Count == 0){ _aimTargets.Add(gameObject.AddComponent<AimTarget>());}
            
            _montageSpeedAttribute = transform.AddComponent<Attribute>();
            _montageSpeedAttribute.resourceTag = GamePlayTag.ActionSpeed;
            _montageSpeedAttribute.maxAmount = 999;
            _montageSpeedAttribute.SetBaseValue(1);
        }

        private void FixedUpdate()
        {
            _animator.SetFloat(_montageSpeedParameter, _montageSpeedAttribute.currentAmount);
            _animator.speed = _owner.timeScale;
        }

        public void SetOwner(Character newOwner){_owner = newOwner;}
        
        public void SetAnimator(Animator newAnimator){ _animator = newAnimator; }
        
        public Animator GetAnimator(){return _animator;}
        
        private void OnAnimatorMove()
        {
            CharacterLocomotionBase locomotion = _owner.Locomotion;
            
            locomotion.AddExtraMove(locomotion.currentAttribute.rootMotionMutiplier * _animator.deltaPosition / _owner.fixedDeltaTime);
            locomotion.InstantRotate(_animator.deltaRotation.eulerAngles.y);
        }
        
        public void TriggerAnimationEvent(int id) { OnAnimationEvent?.Invoke(id); }

        public bool AttachTransformToSocket(Transform t, string socketName)
        {
            GameObject socket = null;

            foreach (Transform child in transform)
            {
                socket = AttachTransformToSocketRec(t, socketName, child);
                if(socket) break;
            }
            
            if (socket == null) return false;
            
            t.parent = socket.transform;
            t.transform.localPosition = Vector3.zero;
            t.transform.localRotation = Quaternion.identity;
            return true;
        }

        GameObject AttachTransformToSocketRec(Transform t, string socketName, Transform current)
        {
            if(current.gameObject.name == socketName) {return current.gameObject;}
            
            foreach (Transform child in current)
            {
                GameObject socket = AttachTransformToSocketRec(t, socketName, child);
                if (socket) return socket;
            }

            return null;
        }
    }
}