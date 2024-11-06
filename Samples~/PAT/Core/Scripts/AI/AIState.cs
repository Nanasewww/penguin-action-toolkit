

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    public class AIState: MonoBehaviour
    {
        [FormerlySerializedAs("_stateTrigger")] [SerializeField] protected GamePlayTag _stateInput = GamePlayTag.None;
        protected float timeInState;

        protected AiBrain _brain;
        protected Character _character;
        
        public virtual void OnEnter(AiBrain b)
        {
            timeInState = 0;
            _brain = b;
            _character = _brain.character;
        }

        public virtual void OnTick()
        {
            //todo: must be removed or AI rework
            _character.TriggerStateWithTag(_stateInput);
            
            timeInState += Time.deltaTime;
            if (!_brain.target) _brain.ReturnIdle();
        }

        public virtual void OnExit()
        {
            
        }


    }
}