using System;
using UnityEngine;

namespace PAT
{
    public class AIAction: AIState
    {
        protected enum DirMode
        {
            facing,
            targetDir
        }
        
        [Header("Locomotion Attributes")] 
        [SerializeField] protected Vector3 relativeDir = new Vector3(0, 0, 1);
        [SerializeField] protected DirMode _dirMode = DirMode.targetDir;
        
        [Header("Cool Down Attributes")]
        [SerializeField] protected float unitCost = 0f;
        [SerializeField] protected float maxCost = 1f;
        [SerializeField] protected float costRecoverSpeed = 1f;
        
        [Space]
        [SerializeField] protected AIState nextState;
        [SerializeField] protected bool canSwapTarget = true;

        protected float _currentCost = 0;
        protected bool _entered = false;
        
        public override void OnEnter(AiBrain b)
        {
            base.OnEnter(b);

            if (!CoolDown()) { _brain.ReturnIdle(); return; }

            //===Set up Attributes and try to enter===//

            Vector3 dir = relativeDir;
            _character.Locomotion.SetDirectionBaseOnFacing(dir);
            
            ActionState state = _character.TriggerStateWithTag(_stateInput); 
            
            //Case fail to act
            if (!state) { _brain.ReturnIdle(); return; }

            _entered = true;
            _currentCost += unitCost;
            _character.onStateChange += OnAttackEnd;
        }
        

        public override void OnTick()
        {
            timeInState += Time.deltaTime;
        }

        public override void OnExit()
        {
            base.OnExit();

            _entered = false;
        }

        public virtual void OnAttackEnd(ActionState newState, ActionState oldState)
        {
            _character.onStateChange -= OnAttackEnd;
            if(nextState) _brain.ChangeState(nextState);
            else _brain.ReturnIdle();
        }

        public void Update()
        {
            if (!_entered) _currentCost -= costRecoverSpeed * Time.deltaTime;
            _currentCost = Mathf.Max(0, _currentCost);
        }

        #region Helper

        public bool CoolDown()
        {
            return _currentCost <= maxCost;
        }

        #endregion
    }
}