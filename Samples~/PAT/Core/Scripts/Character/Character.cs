using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    public class  Character : PATComponent
    {
        [FormerlySerializedAs("_characterModle")]
        [Header("Model")]
        [SerializeField] protected GameObject _characterModel;
        
        [Header("Comopnents")] 
        [SerializeField] private CharacterLocomotionBase locomotion;
        [SerializeField] private ModelHandler _modelHandler;

        [Header("States")] 
        [SerializeField] protected GameObject _moveSetObject;
        [SerializeField] protected MainMoveSet _mainMoveSet;
        [SerializeField] protected List<MoveSet> _stackMoveSets;
        [SerializeField] protected ActionState _currentState;

        [Header("Movement")] 
        [SerializeField] protected Vector3 _inputDirection;

        public event Action<ActionState, ActionState> onStateChange;

        public event Action<MoveSet> onMoveSetLoad;
        public event Action<MoveSet> onMoveSetUnLoad;

        protected Attribute timeAttribute;

        #region Getter
        public GameObject characterModel { get { return _characterModel; } }
        public ActionState currentState { get { return _currentState; } }
        public List<MoveSet> currenMoveSets { get { return _stackMoveSets; } }
        public MainMoveSet mainMoveSet { get { return _mainMoveSet; } }
        
        public GameObject moveSetObject { get { return _moveSetObject; } }

        public List<MoveSet> stackMoveSet {get { return _stackMoveSets; } }
        
        public CharacterLocomotionBase Locomotion { get { return locomotion; } set { locomotion = value; } }
        public ModelHandler modelHandler { get { return _modelHandler; } set { _modelHandler = value; } }
        public Vector3 inputDirection { get { return _inputDirection; } set { _inputDirection = value; }}
        public float timeScale {get {return timeAttribute.currentAmount; } }
        public float deltaTime {get {return Time.deltaTime * timeScale;}}
        public float fixedDeltaTime {get{return Time.fixedDeltaTime * timeScale;}}

        #endregion


        protected override void Awake()
        {
            base.Awake();
            
            if (!_characterModel) {_characterModel = GetComponentInChildren<Animator>().gameObject;}
            
            if (!locomotion) { Locomotion = GetComponent<CharacterLocomotionBase>(); }
            if(modelHandler == null) { modelHandler = _characterModel.GetComponent<ModelHandler>(); }
            if(modelHandler == null) { modelHandler = _characterModel.AddComponent<ModelHandler>(); }
            if (!_moveSetObject) _moveSetObject = gameObject;

            timeAttribute = gameObject.AddComponent<Attribute>();
            timeAttribute.resourceTag = GamePlayTag.TimeScale;
            timeAttribute.SetBaseValue(1f);
            timeAttribute.maxAmount = Mathf.Infinity;
            timeAttribute.minAmount = -Mathf.Infinity;
        }

        protected override void Start()
        {
            base.Start();
            
            if (_mainMoveSet != null) { ChangeBaseMoveSet(_mainMoveSet); }

            //This could help initialize with prefab
            if (_stackMoveSets != null)
            {
                List<MoveSet> copyList = _stackMoveSets.ToList();
                _stackMoveSets.Clear();
                for (int i = 0; i < copyList.Count; i++)
                {
                    LoadMoveSet(copyList[i]);
                }
            }
        }

        public float CharacterFixedDeltaTime()
        {
            return Time.fixedDeltaTime * timeAttribute.currentAmount;
        }

        protected override void Update()
        {
            base.Update();
            
            if (_currentState)
            {
                _currentState.OnTick(deltaTime);
                _currentState.CheckExit();
            }

            //===Process Triggered States===//
            foreach (ActionState state in _mainMoveSet.owningStates)
            {
                if (!state.Triggered()) continue;

                bool result = ChangeState(state);
                if (result) { break; }
            }

            for (int i = _stackMoveSets.Count - 1; i >= 0; i--)
            {
                foreach (ActionState state in _stackMoveSets[i].owningStates)
                {
                    if (!state.Triggered()) continue;

                    bool result = ChangeState(state);
                    if (result) { break; }
                }
            }
        }


        public virtual MainMoveSet ChangeBaseMoveSet(MainMoveSet newSet)
        {
            //If the moveset is a prefab load it
            if (newSet.transform.parent == null)
            {
                newSet = Instantiate(newSet.gameObject, moveSetObject.transform).GetComponent<MainMoveSet>();
                if(!newSet) {Debug.Log("This no valid moveset found"); return null;}
            }
            
            onMoveSetUnLoad?.Invoke(_mainMoveSet);
            _mainMoveSet.Unload();
            
            _mainMoveSet = newSet;
            _mainMoveSet.Initialization(this);
            ChangeState(newSet.idleState);

            //Cast Event
            onMoveSetLoad?.Invoke(newSet);
            _mainMoveSet.Load();

            return _mainMoveSet;
        }

        public virtual MoveSet LoadMoveSet(MoveSet newSet)
        {
            if (newSet.transform.parent == null)
            {
                newSet = Instantiate(newSet.gameObject, moveSetObject.transform).GetComponent<MoveSet>();
                if(!newSet) {Debug.Log("This no valid moveset found"); return null;}
            }
            
            newSet.Initialization(this);
            _stackMoveSets.Add(newSet);

            onMoveSetLoad?.Invoke(newSet);
            newSet.Load();

            return newSet;
        }

        public virtual void UnLoadMoveSet(MoveSet oldSet)
        {
            _stackMoveSets.Remove(oldSet);
            
            onMoveSetUnLoad?.Invoke(oldSet);
            oldSet.Unload();
        }

        public virtual bool ChangeState(ActionState newState)
        {
            if (!enabled) return false;
            
            ActionState oldState = _currentState;

            if (!newState.Validation()) return false;
            //if (_currentState == newState) return false;

            //If validation succeed
            if (_currentState != null) _currentState.OnExit();
            _currentState = newState;
            _currentState.OnEnter();

            //Cast Event
            if (onStateChange != null) onStateChange.Invoke(newState, oldState);

            return true;
        }
        
        
        public override void ProcessDeath()
        {
            if (_mainMoveSet.deathState) ChangeState(_mainMoveSet.deathState);
            base.ProcessDeath();
        }

        #region Helper

        //todo: super costly method, a lot foreach and check Triggered it self is costly 
        public ActionState TriggerStateWithTag(GamePlayTag target)
        {
            if (!enabled) return null;
            
            tagContainer.inputTags.Add(target);
            
            foreach (ActionState state in _mainMoveSet.owningStates)
            {
                if (!state.Triggered()) continue;
                 
                bool result = ChangeState(state);
                if (result)
                {
                    tagContainer.inputTags.Remove(target);
                    return state;
                }
            }

            for (int i = _stackMoveSets.Count - 1; i >= 0; i--)
            {
                foreach (ActionState state in _stackMoveSets[i].owningStates)
                {
                    if (!state.Triggered()) continue;

                    bool result = ChangeState(state);
                    if (result)
                    {
                        tagContainer.inputTags.Remove(target);
                        return state;
                    }
                }
            }
            tagContainer.inputTags.Remove(target);
            return null;
        }

        public void EndCurrentState()
        {
            if (!enabled) return;

            if (!_mainMoveSet) return;
            if (!_mainMoveSet.idleState) return;
            if (!_currentState) return;
            
            _currentState.EndCurrentState();
        }
        
        public List<ActionState> GetAllActions()
        {
            List<ActionState> actions = new List<ActionState>();

            foreach (ActionState state in _mainMoveSet.owningStates) actions.Add(state);
            for (int i = _stackMoveSets.Count - 1; i >= 0; i--)
            {
                foreach (ActionState state in _stackMoveSets[i].owningStates) actions.Add(state);
            }
            
            return actions;
        }
        
        public List<ActionState> GetAllPossibleActions()
        {
            List<ActionState> actions = new List<ActionState>();

            foreach (ActionState action in GetAllActions())
            {
                if(action.Validation()) actions.Add(action);
            }
            
            return actions;
        }

        #endregion
    }
}
