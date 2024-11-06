using UnityEngine;
using UnityEngine.Serialization;
using System;


namespace PAT
{
    [Serializable]
    public class AnimationInfo
    {
        [Tooltip("The referencing animator")]
        public RuntimeAnimatorController referenceAnimator;
        [Tooltip("Name of State that contains the desired animation clip inside the Animator")]
        public string stateName;
        [Tooltip("Duration of cross-fade when the animation start")]
        public float fadeTime;
        [Tooltip("Fade out duration, target is a state named “Empty” in the same layer")]
        public float fadeOutTime;
        [Tooltip("Layer index of the animation state in the animator (an increasing integer starts from 0)")]
        public int layer;
    }
    
    public class AnimationMontageMod: StateModifier
    {
        [SerializeField] protected AnimationInfo _info;
        [SerializeField] protected bool _keepPlayOnExit = false;
        [SerializeField] protected bool _exitOnMontageEnd = false;

        protected float _normalizedTimeInState;
        public AnimationInfo info
        {
            get { return _info; }
            set { _info = value; }
        }

        public bool exitOnMontageEnd
        {
            get { return _exitOnMontageEnd; }
            set { _exitOnMontageEnd = value; }
        }

        void Reset()
        {
            _info = new AnimationInfo();
            mode = ModifierMode.ByTimeInState;
        }

        private void Awake()
        {
            if (_info == null) _info = new AnimationInfo();
        }

        public override void LateOnUpdate(Character controller)
        {
            base.LateOnUpdate(controller);

            Animator animator = characterController.modelHandler.GetAnimator();
            if(!animator) return;

            //todo: this is almost good, but there might be inaccuracy due to fade in
            //possible fix is sync with normalized time after fade in
            _normalizedTimeInState += Time.deltaTime/animator.GetCurrentAnimatorStateInfo(_info.layer).length;

            if (_exitOnMontageEnd && timeInState > _info.fadeTime 
                                  && animator.GetCurrentAnimatorStateInfo(_info.layer).IsName(_info.stateName)
                                  && _normalizedTimeInState > 1)
            {
                characterController.EndCurrentState();
            }
        }

        public override void BeginEvent()
        {
            base.BeginEvent();

            Animator animator = characterController.modelHandler.GetAnimator();
            if(!animator) {Debug.LogWarning("no animator found in character model"); return;}
            if(_info.stateName == "") return;
            
            if (animator.GetCurrentAnimatorStateInfo(_info.layer).IsName(_info.stateName))
            {
                animator.Play("Empty", _info.layer);
                animator.Play(_info.stateName,_info.layer, 0);
            }
            animator.CrossFadeInFixedTime(_info.stateName, _info.fadeTime, _info.layer);
            _normalizedTimeInState = 0;
        }

        public override void EndEvent()
        {
            base.EndEvent();
            
            if(_keepPlayOnExit) return;
            
            Animator animator = characterController.modelHandler.GetAnimator();
            if(!animator) return;

            
            animator.CrossFadeInFixedTime("Empty", _info.fadeOutTime, _info.layer);
        }
    }
}