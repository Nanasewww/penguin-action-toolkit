using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace PAT
{
    public class AIIdle: AIState
    {
        [SerializeField] protected float _detectionDistance = 15f;
        [SerializeField] protected float _lostTargetDistance = 30f;
        [SerializeField] protected float _combatDistance = 5f;
        [SerializeField] protected AIState _combatState;
        [SerializeField] public float _stopAtLeaderDistance = 3f;
        
        public override void OnEnter(AiBrain b)
        {
            base.OnEnter(b);
            
            _character.Locomotion.SetDirectionBaseOnFacing(Vector3.zero);
        }

        public override void OnTick()
        {
            base.OnTick();
            
            if (_brain.target)
            {
                ProcessFollowTarget();
            }
            else
            {
                ProcessPatrol();
            }
        }

        void ProcessPatrol()
        {
            List<PATComponent> allEnemies = _brain.GetOtherTeamCharacters();
            float minDist = _detectionDistance;
            PATComponent newTarget = null;

            foreach (var enemy in allEnemies)
            {
                float dis = Vector3.Magnitude(_character.transform.position - enemy.transform.position);
                    
                if (!(dis < minDist)) continue;
                newTarget = enemy;
                minDist = dis;
            }

            _brain.target = newTarget;
            
            _character.Locomotion.SetDirectionBaseOnFacing(Vector3.zero);
            
            //If no enemy, return to leader
            if(_brain.target || !_brain.Leader) return;
            
            _brain.target = _brain.Leader;
            if(_brain.DistanceToTarget() > _stopAtLeaderDistance) _character.Locomotion.SetCurrentMoveDirection(_brain.DirectionToTarget());
            _brain.target = null;
        }

        void ProcessFollowTarget()
        {
            _character.Locomotion.SetCurrentMoveDirection(_brain.DirectionToTarget());
            
            if (_brain.DistanceToTarget() <= _combatDistance)
            {
                _brain.ChangeState(_combatState);
            }
            
            if (_brain.DistanceToTarget() > _lostTargetDistance)
            {
                _brain.target = null;
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _detectionDistance);
            Gizmos.color = Color.yellow;
        }
    }
}