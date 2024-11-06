using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace PAT
{
    public class ExitOnInputMissMod : StateModifier
    {
        [Serializable] 
        public class ReleaseResult
        {
            public float time;
            public UnityEvent onRelease;
            public ActionState changeToState;
            public GamePlayTag triggerState = GamePlayTag.None;
            public List<Effect> addEffects;
            
        }

        [SerializeField] protected GamePlayTag _inputTag = GamePlayTag.None;
        [SerializeField] private List<ReleaseResult> _releaseResults;

        #region Getter
        
        public List<ReleaseResult> releaseResults
        {
            get { return _releaseResults; }
            set { _releaseResults = value; }
        }

        public GamePlayTag inputTag
        {
            get { return _inputTag; }
            set { _inputTag = value; }
        }

        #endregion

        public override void LateOnUpdate(Character controller)
        {
            base.EarlyOnUpdate(controller);
            
            if(_inputTag == GamePlayTag.None) return;
            
            ProcessRelease(controller);
        }

        protected void ProcessRelease(Character controller)
        {
            if (characterController.tagContainer.inputTags.Contains(_inputTag))return;

            if (_releaseResults.Count <= 0) { controller.EndCurrentState(); Debug.Log("release"); return;}
            
            if (timeInState <= 0.05f) return;
            
            foreach (var r in _releaseResults)
            {
                if(r.time < timeInState) continue;
                
                if (r.addEffects != null)
                {
                    foreach (var effect in r.addEffects)
                    {
                        controller.AddEffect(effect);
                    }
                }
                if (r.changeToState)
                {
                    controller.ChangeState(r.changeToState);
                    return;
                }
                if (r.triggerState != GamePlayTag.None)
                {
                    controller.TriggerStateWithTag(r.triggerState);
                    return;
                }
                
                break;
            }
            
            controller.EndCurrentState();
        }
    }
}
