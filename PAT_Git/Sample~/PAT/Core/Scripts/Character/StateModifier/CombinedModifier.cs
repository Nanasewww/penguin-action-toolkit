using System;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class CombinedModifier : StateModifier
    {
        protected List<StateModifier> stateModifiers = new List<StateModifier>();

        public override void Initialization(Character controller)
        {
            base.Initialization(controller);
            foreach (StateModifier modifier in stateModifiers)
            {
                modifier.Initialization(controller);
            }
        }

        public override void OnEnter(Character controller)
        {
            base.OnEnter(controller);
            foreach (StateModifier modifier in stateModifiers)
            {
                modifier.OnEnter(controller);
            }
        }

        public override void EarlyOnUpdate(Character controller)
        {
            base.EarlyOnUpdate(controller);
            foreach (StateModifier modifier in stateModifiers)
            {
                modifier.EarlyOnUpdate(controller);
            }
        }

        public override void LateOnUpdate(Character controller)
        {
            base.LateOnUpdate(controller);
            foreach (StateModifier modifier in stateModifiers)
            {
                modifier.LateOnUpdate(controller);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnExit(Character controller)
        {
            base.OnExit(controller);
            foreach (StateModifier modifier in stateModifiers)
            {
                modifier.OnExit(controller);
            }
        }

        public override bool Validate(Character controller)
        {
            bool valid = true;
            valid &= base.Validate(controller);
            foreach (StateModifier modifier in stateModifiers)
            {
                valid &= modifier.Validate(controller);
            }

            return valid;
        }
    }
}