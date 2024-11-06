using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace PAT
{
    public enum ModifierMode
    {
        ByAnimationEvent,
        ByTimeInState
    }
    [Serializable]
    
    public class StateModifier : MonoBehaviour
    {
        [Tooltip("For you to track what this modifier is for")]
        [FormerlySerializedAs("mfName")] public string modName;
        
        protected float timeInState;
        protected Character characterController;
        
        [Serializable]
        public struct Events
        {
            public UnityEvent onBegin;
            public UnityEvent onEnd;
        }
        
        
        public ModifierMode mode;
        [Tooltip("If set to -1, modifier will be triggered as soon as the State is entered")]
        [SerializeField] protected int _beginIndex = 1;
        [Tooltip("If set to -1, modifier will not be turned off until the State is exited.")]
        [SerializeField] protected int _endIndex = 2;
        
        [Tooltip("If set to -1, modifier will be triggered as soon as the State is entered")]
        [SerializeField] protected float _beginTime = 0;
        [Tooltip("If set to -1, modifier will not be turned off until the State is exited.")]
        [SerializeField] protected float _endTime = -1;
        [SerializeField] protected Events _events = new Events(){ onBegin = new UnityEvent(), onEnd = new UnityEvent()};

        protected bool started;
        protected bool ended;


        #region Getter
        public int beginIndex { get { return _beginIndex; } set { _beginIndex = value; } }
        public int endIndex { get { return _endIndex; } set { _endIndex = value; } }
        public float beginTime { get { return _beginTime; } set { _beginTime = value; } }
        public float endTime { get { return _endTime; } set { _endTime = value; } }
        #endregion

        private void Reset()
        {
            _beginIndex = 1;
            _endIndex = 2;
            _beginTime = 0;
            _endTime = -1;

            mode = ModifierMode.ByTimeInState;
        } 
        
        public virtual void Initialization(Character controller)
        {
            characterController = controller;
        }
        
        public virtual void OnEnter(Character controller)
        {
            timeInState = 0f;
            if (mode == ModifierMode.ByAnimationEvent)
            {
                controller.modelHandler.OnAnimationEvent += OnBeginAnimationEvent;
                controller.modelHandler.OnAnimationEvent += OnEndAnimationEvent;
            }

            started = false;
            ended = false;

            if (_beginIndex < 0) { BeginEvent(); }
        }

        public virtual void EarlyOnUpdate(Character controller)
        {
            //todo: shall we make this dynamic one day?
            timeInState += Time.deltaTime;
            //Only execute the following for time in state mode
            if (mode != ModifierMode.ByTimeInState) return;

            if (!started && timeInState >= _beginTime) { BeginEvent(); }
            if (!ended && started && _endTime > 0 && timeInState >= _endTime) { EndEvent(); }
        }

        public virtual void LateOnUpdate(Character controller)
        {

        }

        public virtual void OnExit(Character controller)
        {
            //If the state is kind of interrupt, clean it up
            if (!ended) { EndEvent(); }

            if (mode == ModifierMode.ByAnimationEvent)
            {
                controller.modelHandler.OnAnimationEvent -= OnBeginAnimationEvent;
                controller.modelHandler.OnAnimationEvent -= OnEndAnimationEvent;
            }
        }

        public virtual bool Validate(Character controller)
        {
            return true;
        }
        
        void OnBeginAnimationEvent(int id)
        {
            if (id != _beginIndex) return;

            BeginEvent();
        }

        void OnEndAnimationEvent(int id)
        {
            if (id != _endIndex) return;
            if (!started) return;//never end before started, to prevent unexpected behavior

            EndEvent();
        }
        
        public virtual void BeginEvent() { started = true; _events.onBegin.Invoke();}

        public virtual void EndEvent() { ended = true; _events.onEnd.Invoke();}

    }
}
