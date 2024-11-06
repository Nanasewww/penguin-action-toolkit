using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace PAT
{
    public class AiBrain : MonoBehaviour
    {
        public static List<AiBrain> brains;
        
        [SerializeField] protected Character _character;
        [SerializeField] protected NavMeshAgent _agent;
        [SerializeField] protected AIState _idleState;
        [SerializeField] protected AIState _afterHurtState;
        [SerializeField] protected AIState _currentState;

        [SerializeField] protected PATComponent _target;
        [SerializeField] protected Character _leader;

        public Character character
        {
            get { return _character; }
        }
        
        public PATComponent target
        {
            get { return _target; }
            set { _target = value; }
        }

        public Character Leader
        {
            get { return _leader; }
            set { _leader = value; }
        }
        

        private void Awake()
        {
            if (brains == null) brains = new List<AiBrain>();
            brains.Add(this);
            
            if (!_agent) _agent = GetComponent<NavMeshAgent>();

            if (!_character) _character = GetComponent<Character>();
            if (!_character) _character = GetComponentInParent<Character>();
            if (!_character) _character = GetComponentInChildren<Character>();
        }

        public void Start()
        {
            ChangeState(_idleState);

            if (!_character) return;
            _character.onStateChange += OnCharacterStateChange;
            _character.healthAttribute.OnModApplied += OnCharacterHurt;
        }

        public void OnDestroy()
        {
            if (!_currentState) return;
            _character.onStateChange -= OnCharacterStateChange;
            _character.healthAttribute.OnModApplied  -= OnCharacterHurt;

            brains.Remove(this);
        }

        public void ChangeState(AIState newState)
        {
            if(!enabled) return;
            if(!newState) return;
            
            _currentState?.OnExit();
            _currentState = newState;
            _currentState.OnEnter(this);
        }

        public void Update()
        {
            if(!_character.tagContainer.CheckForTag(GamePlayTag.Hurt))_currentState?.OnTick();
            if (target && !_target.enabled) target = null;
            
            if (_agent)
            {
                // Disable auto-braking
                _agent.autoBraking = false;

                // Disable NavMeshAgent's automatic movement
                _agent.updatePosition = false;
                _agent.updateRotation = false;
                _agent.speed = _character.Locomotion.currentAttribute.MoveSpeed;
                _agent.angularSpeed = _character.Locomotion.currentAttribute.rotationSpeed;

                _agent.nextPosition = transform.position;
            }

            if (_target)
            {
                Vector3 direction = DirectionToTarget();
                direction.y = 0;
                _character.Locomotion.SetCurrentRotateDirection(direction);
            }
            else
            {
                _character.Locomotion.SetCurrentMoveDirection(_character.Locomotion.currentMoveDirection);
            }
        }

        #region Listener

        public void OnCharacterStateChange(ActionState newState, ActionState oldState)
        {
            if (newState == _character.mainMoveSet.deathState)
            {
                Destroy(gameObject);
                return;
            }
            
            if (oldState && oldState.mainTags.Contains(GamePlayTag.Hurt) && !newState.mainTags.Contains(GamePlayTag.Hurt))
            {
                ChangeState(_afterHurtState);
                return;
            }
        }

        void OnCharacterHurt(EffectModValue mod)
        {
            if (mod.ownerEffect.source)
            {
                _target = mod.ownerEffect.source;
            }
        }
        #endregion

        
        #region helper
        public float DistanceToTarget()
        {
            if (!_target) return float.PositiveInfinity;
            if (!_agent) return Vector3.Distance(transform.position, _target.transform.position);
            
            _agent.destination = target.transform.position;
            return _agent.remainingDistance;
        }

        public Vector3 DirectionToTarget()
        {
            if (!_target) return Vector3.positiveInfinity;
            
            if (!_agent) return (_target.transform.position - transform.position).normalized;

            _agent.destination = target.transform.position;
            return (_agent.steeringTarget - transform.position).normalized;
        }

        public void ReturnIdle()
        {
            if(_currentState != _idleState) ChangeState(_idleState);
        }
        
        public List<PATComponent> GetOtherTeamCharacters()
        {
            return PATComponent.allInstance.Where(c => c.team != _character.team).ToList();
        }
        
        #endregion

    }
}
