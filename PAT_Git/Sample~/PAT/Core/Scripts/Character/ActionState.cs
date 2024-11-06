using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace PAT
{

    [Serializable]
    public class ActionState : MonoBehaviour
    {
        
        [Serializable]
        public struct RequirementGroup
        {
            public bool NoRequirement;
            public List<GamePlayTag> _tags;
        }
        
        [Tooltip("When input tag is detected, try to enter this State")]
        [SerializeField] protected GamePlayTag _inputTag = GamePlayTag.None;
        
        [Tooltip("If the State can re-enter itself when all other conditions are met")]
        [SerializeField] protected bool _canRepeat;
        
        [Tooltip("Change to next State (default: Idle) after time. If set to 0, it will not auto exit")]
        [SerializeField] protected float autoExitTime;
        
        
        [FormerlySerializedAs("_grantTags")]
        [Header("Tags")]
        [Tooltip("Tags a character will hold in this State")]
        [SerializeField] protected List<GamePlayTag> _mainTags;
        
        [Tooltip("Character must have these tags to enter this State, outer elements are logically disjunction (or) to each other, inner elements are logically conjunction (and) to each other")]
        [SerializeField] protected List<RequirementGroup> _requireTags;
        
        [Tooltip("Cannot enter a State with listed tags while in this State")]
        [SerializeField] protected List<GamePlayTag> _prohibitTags;
        
        [Tooltip("This State cannot be entered if character is holding listed tags")]
        [SerializeField] protected List<GamePlayTag> _selfProhibitTags;
        
        
        [Header("Special")]
        [Tooltip("Regardless of the prohibit / require relationship of tags, States listed here are always allowed during this State")]
        [SerializeField] protected List<ActionState> _permitState;
        
        [Tooltip("Force character to enter specified State (default: Idle) if this State ends on its own")]
        [SerializeField] protected ActionState _nextState;


        [Header("Modifiers")]
        [SerializeField] protected List<StateModifier> modifiers;
        [FormerlySerializedAs("additionMFObj")] [SerializeField] protected List<GameObject> additionModObj;

        public float timeInState { get; protected set; }
        protected Character character;
        private ActionState _forceChangeState;


        #region Getter
        public List<ActionState> permitState { get { return _permitState; } }
        public GamePlayTag inputTag { get { return _inputTag; } }
        public List<GamePlayTag> mainTags { get { return _mainTags; } }

        #endregion

        // Initialized by MoveSet.cs
        // owner - Character
        public virtual void Initialization(Character owner)
        {
            character = owner;

            void GetMFS(StateModifier[] attachedModifiers)
            {
                foreach (var modifier in attachedModifiers)
                {
                    if (!modifiers.Contains(modifier)) { modifiers.Add(modifier);}
                }
                
            }
            
            GetMFS(GetComponents<StateModifier>());

            //Check additions
            foreach (GameObject g in additionModObj)
            {
                GetMFS(g.GetComponents<StateModifier>());
            }

            foreach (StateModifier mf in modifiers) { mf.Initialization(character); }

        }

        /// <summary>
        ///
        /// This will tell the character if this state want to entry at this frame
        /// However, the validation still need to passed
        /// Comparing to using change state directly
        /// This way of entry can handle priority better
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool Triggered()
        {
            if (_inputTag!=GamePlayTag.None && character.tagContainer.inputTags.Contains(_inputTag)) return true;
            
            return false;
        }


        /// <summary>
        /// Super crucial Function
        /// Managed how the enter logics functions
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual bool Validation()
        {
            if (!gameObject.activeSelf) return false;
            if (!this.enabled) return false;

            //Rare, but will be helpful
            foreach (StateModifier modifier in modifiers)
            {
                if (!modifier.Validate(character)) return false;
            }

            //High priority
            if (character.currentState && character.currentState.permitState.Contains(this)) return true;

            if (!_canRepeat && character.currentState == this) return false;
            
            //Check for prohibition
            foreach (GamePlayTag t in _mainTags) {if (character.tagContainer.prohibitedTags.Contains(t)) return false;}
            foreach (GamePlayTag t in _selfProhibitTags){ if(character.tagContainer.CheckForTag(t))return false;}
            
            //Check for requirement
            foreach (RequirementGroup g in _requireTags)
            {
                if (g.NoRequirement) return true;

                bool pass = true;
                foreach (GamePlayTag t in g._tags)
                {
                    if (!character.tagContainer.CheckForTag(t)) pass = false;
                }

                if (pass) return true;
            }
            
            return false;
        }



        public virtual void OnEnter()
        {
            //Debug.Log(controller.gameObject.name + " is entering: "+ gameObject.name);
            
            timeInState = 0;

            foreach (GamePlayTag mainTag in _mainTags) character.tagContainer.stateTags.Add(mainTag);
            foreach (GamePlayTag prohibitTag in _prohibitTags) character.tagContainer.prohibitedTags.Add(prohibitTag);

            foreach (StateModifier modifier in modifiers)
            {
                if (modifier.enabled) { modifier.OnEnter(character); }
            }
        }

        public virtual void OnExit()
        {

            foreach (GamePlayTag t in _mainTags) { character.tagContainer.stateTags.Remove(t); }
            foreach (GamePlayTag t in _prohibitTags) { character.tagContainer.prohibitedTags.Remove(t); }

            foreach (StateModifier modifier in modifiers)
            {
                if (modifier.enabled) { modifier.OnExit(character); }
            }

        }

        public virtual void OnTick(float deltaTime)
        {
            foreach (StateModifier modifier in modifiers)
            {
                if (modifier.enabled) { modifier.EarlyOnUpdate(character); }
                if (modifier.enabled) { modifier.LateOnUpdate(character); }
            }

            timeInState += deltaTime;

            if (autoExitTime > 0 && timeInState >= autoExitTime)
            {
                if (_nextState) ForceChangeState(_nextState);
                else EndCurrentState();
            }
            
            
        }

        public virtual void CheckExit()
        {
            if (_forceChangeState)
            {
                this._permitState.Add(_forceChangeState);
                character.ChangeState(_forceChangeState);
                this._permitState.Remove(_forceChangeState);
                _forceChangeState = null;
            } 
        }
        

        #region Helpers

        /// <summary>
        /// 
        /// This is a helper, to change the current state into a new state
        /// Even if the new state do not have current state is prerequisite
        /// Or current state did not permitted it
        /// 
        /// </summary>
        /// <param name="newState"></param>
        public virtual void ForceChangeState(ActionState newState)
        {
            _forceChangeState = newState;
        }

        public virtual void EndCurrentState()
        {
            if (_nextState) ForceChangeState(_nextState);
            else ForceChangeState(character.mainMoveSet.idleState);
        }

        #endregion
        
    }
}
