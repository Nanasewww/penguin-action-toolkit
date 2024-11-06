using System;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class MoveSet : MonoBehaviour
    {
        [Header("States")]
        [SerializeField] protected bool _autoCollect = true;
        [SerializeField] protected List<ActionState> _owningStates;

        [Header("Attributes")] protected Character ownerCharacter;
        
        
        public event Action<Character> onInitialized;
        
        public event Action onLoaded;
        
        public event Action onUnloaded;
        
        #region Getter
        public List<ActionState> owningStates { get { return _owningStates; } }
        #endregion
        

        /// <summary>
        /// Mainly initializing the owning states
        /// Will auto collect from child if toggled
        /// </summary>
        public virtual void Initialization(Character controller)
        {
            if (ownerCharacter == controller) return;
            
            ownerCharacter = controller;
            
            if (_autoCollect)
            {
                ActionState[] childStates = GetComponentsInChildren<ActionState>();
                foreach (ActionState childState in childStates)
                {
                    if (!_owningStates.Contains(childState)) _owningStates.Add(childState);
                }
            }

            foreach (ActionState state in _owningStates) state.Initialization(controller);
            
            onInitialized?.Invoke(controller);
        }

        public virtual void Load()
        {
            onLoaded?.Invoke();
        }

        public virtual void Unload()
        {
            onUnloaded?.Invoke();
        }
    }

}