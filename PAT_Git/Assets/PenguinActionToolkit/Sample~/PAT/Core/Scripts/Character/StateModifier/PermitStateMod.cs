using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class PermitStateMod: StateModifier
    {
        [SerializeField] protected List<ActionState> _states;

        public override void BeginEvent()
        {
            base.BeginEvent();
            foreach (var s in _states)
            {
                characterController.currentState.permitState.Add(s);
            }
        }

        public override void EndEvent()
        {
            base.EndEvent();
            foreach (var s in _states)
            {
                characterController.currentState.permitState.Remove(s);
            }
        }
    }
}