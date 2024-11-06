using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    public class AIHoldAction: AIAction
    {
        [FormerlySerializedAs("_duration")] [SerializeField] protected float _chargeDuration = 1f;
        public override void OnEnter(AiBrain b)
        {
            base.OnEnter(b);
            
            if(!_entered) return;
            
            _character.onStateChange -= OnAttackEnd;
            _character.tagContainer.inputTags.Add(_stateInput);
            StartCoroutine(HoldComplete());
        }

        public override void OnAttackEnd(ActionState newState, ActionState oldState)
        {
            base.OnAttackEnd(newState, oldState);
            _character.tagContainer.inputTags.Remove(_stateInput);
            StopAllCoroutines();
        }

        IEnumerator HoldComplete()
        {
            yield return new WaitForSeconds(_chargeDuration);
            _character.tagContainer.inputTags.Remove(_stateInput);

            yield return new WaitForSeconds(0.1f);
            _character.onStateChange += OnAttackEnd;
        }
        

    }
}