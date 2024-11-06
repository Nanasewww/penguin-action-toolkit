using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    public class AICombatStance: AIState
    {
        [Serializable]
        protected struct AttemptUnit
        {
            public AIState state;
            public float maxDistance;
            public float minDistance;
            public int chance;
        }
        
        [Serializable]
        protected struct ReactUnit
        {
            public GamePlayTag targetTag;
            public AIState state;
            public float maxDistance;
            public int chance;
        }

        [Header("Attributes")] 
        [SerializeField] protected float _timeBeforeAttempt = 1f;
        [SerializeField] protected float _timeBeforeReact = 0.3f;
        [SerializeField] protected Vector3 _moveVector = new Vector3(1f, 0f, 1f);
        [SerializeField] protected float _moveScaler = 0.5f;
        [SerializeField] protected float _minDistance = 2;
        [SerializeField] protected float _exitDistance = 10;
        
        [Header("Moves")]
        [SerializeField] protected List<AttemptUnit> _attemptUnits;
        [SerializeField] protected List<ReactUnit> _reactUnits;
        
        [Header("Other States")] 
        [SerializeField] protected AIState _appraochState;
        [SerializeField] protected AIState _exitState;

        protected bool _reacted = false;

        public override void OnEnter(AiBrain b)
        {
            base.OnEnter(b);

            if (!_brain.target)
            {
                _brain.ReturnIdle();
                return;
            }
            _reacted = false;
        }

        public override void OnTick()
        {
            base.OnTick();

            //Exit Condition
            if (_brain.DistanceToTarget() > _exitDistance)
            {
                _brain.ChangeState(_exitState);
                return;
            }
            
            //Move Character On Need
            Vector3 move = _moveVector.normalized * _moveScaler;
            if (_brain.DistanceToTarget() <= _minDistance) move = new Vector3(move.x, move.y, 0f);
            _character.Locomotion.SetDirectionBaseOnFacing(move);
            
            CheckForReact();
            if(timeInState < _timeBeforeAttempt) return;
            AttemptAttack();
        }

        public void AttemptAttack()
        {
            if(_reacted) return;
            
            List<AttemptUnit> validUnits = _attemptUnits.Where(a => a.maxDistance >= _brain.DistanceToTarget()).ToList();
            validUnits = validUnits .Where(a => a.minDistance < _brain.DistanceToTarget()).ToList();
            
            int totalScore = 0;
            foreach (var unit in validUnits) { totalScore += unit.chance; }

            int rand = UnityEngine.Random.Range(0, totalScore) + 1;
            int currentStack = 0;
            foreach (var unit in validUnits)
            {
                currentStack += unit.chance;
                if(currentStack < rand) continue;
                if(unit.state.GetType() == typeof(AIAction) && !((AIAction)unit.state).CoolDown()) break;
                
                _brain.ChangeState(unit.state);
                return;
            }
        }

        public void CheckForReact()
        {
            if(_reacted) return;
            
            List<ReactUnit> validUnits = _reactUnits.Where(a => a.maxDistance >= _brain.DistanceToTarget()).ToList();
            validUnits = validUnits.Where(a => _brain.target.tagContainer.CheckForTag(a.targetTag)).ToList();
            
            int totalScore = 0;
            foreach (var unit in validUnits) { totalScore += unit.chance; }

            int rand = UnityEngine.Random.Range(0, totalScore) + 1;
            int currentStack = 0;
            foreach (var unit in validUnits)
            {
                currentStack += unit.chance;
                if(currentStack < rand) continue;
                if(unit.state.GetType() == typeof(AIAction) && !((AIAction)unit.state).CoolDown()) break;
                
                StartCoroutine(ReactCoroutine(unit.state));
                return;
            }
        }

        IEnumerator ReactCoroutine(AIState state)
        {
            _reacted = true;
            
            yield return new WaitForSeconds(_timeBeforeReact);

            _brain.ChangeState(state);
            _reacted = false;
        }
    }
}