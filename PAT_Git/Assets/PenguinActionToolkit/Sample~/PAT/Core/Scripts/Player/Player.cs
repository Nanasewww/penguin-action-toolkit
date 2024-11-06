using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
/*using UnityEditor.SceneManagement;*/
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;

namespace PAT
{
    public class Player : MonoBehaviour
    {
        public static List<Player> Players;
        public int PlayerIndex = -1;

        [FormerlySerializedAs("_playerCharacter")] [SerializeField]
        protected Character _character;

        [SerializeField] protected Camera _playerCam;
        [SerializeField] protected List<InputUnit> _inputUnits;

        protected PlayerInput playerInput;

        protected PlayerLocomotionInputBase _locomotionInput;
        protected readonly HashSet<PlayerInputAddOn> _characterInputAddOns = new HashSet<PlayerInputAddOn>();

        /// <summary>Invokes when new character assigned to this player </summary>
        public event Action<Character> OnCharacterChange;

        //protected bool initialize = false;

        #region Getter

        public Character character
        {
            get { return _character; }
        }

        public Camera playerCam
        {
            get { return _playerCam; }
        }

        public HashSet<PlayerInputAddOn> characterInputAddOns
        {
            get { return _characterInputAddOns; }
        }

        #endregion

        public virtual void Awake()
        {
            if (Players == null) Players = new List<Player>();
            Players.Add(this);

            _locomotionInput = GetComponentInChildren<PlayerLocomotionInputBase>() ??
                               gameObject.AddComponent<PlayerLocomotionInput3D>();
        }

        private void Start()
        {
            if (!_playerCam) _playerCam = Camera.main;
            ChangeCharacter(_character);
        }

        private void OnDestroy()
        {
            Players.Remove(this);
        }

        /// <summary>
        /// The update throughs locomotion inputs into player
        /// </summary>
        private void Update()
        {
            if (!_character) return;
            if (!_playerCam) return;

            if (PlayerIndex != -1 && playerInput == null)
            {
                //todo: fix this, noway using find objects of type in update
                foreach (var input in FindObjectsOfType<PlayerInput>())
                {
                    if (input.playerIndex == PlayerIndex)
                    {
                        playerInput = input;
                    }
                }

                return;
            }

            _locomotionInput.ProcessLocomotionInputs(this);
            ProcessActionInputs();
        }

        private void ProcessActionInputs()
        {
            HashSet<GamePlayTag> processedTags = new HashSet<GamePlayTag>();

            foreach (PlayerInputAddOn addOn in _characterInputAddOns)
            {
                InputUnit unit = addOn.inputUnit;

                //Avoid overlap in singular tag
                if (!processedTags.Add(unit._inputTag)) continue;

                //Process Logics
                unit.OnTick(this);
                bool result = unit.Triggered();
                List<GamePlayTag> inputTags = _character.tagContainer.inputTags;

                if (result && !inputTags.Contains(unit._inputTag))
                {
                    inputTags.Add(unit._inputTag);
                }

                if (!result && inputTags.Contains(unit._inputTag))
                {
                    inputTags.Remove(unit._inputTag);
                }
            }

            foreach (var unit in _inputUnits)
            {
                //Avoid overlap in singular tag
                if (!processedTags.Add(unit._inputTag)) continue;

                //Process Logics
                unit.OnTick(this);
                bool result = unit.Triggered();
                List<GamePlayTag> inputTags = _character.tagContainer.inputTags;

                if (result && !inputTags.Contains(unit._inputTag))
                {
                    inputTags.Add(unit._inputTag);
                }

                if (!result && inputTags.Contains(unit._inputTag))
                {
                    inputTags.Remove(unit._inputTag);
                }
            }
        }

        void ClearBuffer(ActionState newState, ActionState oldState)
        {
            foreach (var unit in _inputUnits)
            {
                unit.ResetBuffer();
            }
        }

        public void ChangeCharacter(Character newCharacter)
        {
            if (newCharacter == null)
            {
                Debug.LogWarning("No player assigned");
                return;
            }

            UpdateCharacterInputAddOns(_character, newCharacter);
            _character.tagContainer.inputTags.Clear();
            _character.onStateChange -= ClearBuffer;
            _character = newCharacter;
            OnCharacterChange?.Invoke(newCharacter);

            _character.onStateChange += ClearBuffer;
        }

        void UpdateCharacterInputAddOns(Character oldC, Character newC)
        {
            //Remove all old addons
            _characterInputAddOns.Clear();

            //Unsubscrbie listener from older character
            oldC.onMoveSetLoad -= LoadMoveSetAddon;
            oldC.onMoveSetUnLoad -= UnLoadMoveSetAddon;

            //Get all current addons
            PlayerInputAddOn[] newAddOns = newC.GetComponentsInChildren<PlayerInputAddOn>();
            foreach (PlayerInputAddOn newAddon in newAddOns) _characterInputAddOns.Add(newAddon);

            //Subscrbie listener to monitor more addons
            newC.onMoveSetLoad += LoadMoveSetAddon;
            newC.onMoveSetUnLoad += UnLoadMoveSetAddon;
        }

        void LoadMoveSetAddon(MoveSet moveSet)
        {
            PlayerInputAddOn[] newAddOns = moveSet.GetComponentsInChildren<PlayerInputAddOn>();
            foreach (PlayerInputAddOn newAddon in newAddOns) _characterInputAddOns.Add(newAddon);
        }

        void UnLoadMoveSetAddon(MoveSet moveSet)
        {
            PlayerInputAddOn[] oldAddOns = moveSet.GetComponentsInChildren<PlayerInputAddOn>();
            foreach (PlayerInputAddOn newAddon in oldAddOns)
            {
                _characterInputAddOns.Remove(newAddon);
            }
        }

        /// <summary>
        /// Copied from unity Cinemachine Input Provider
        /// </summary>
        /// <param name="actionRef"></param>
        /// <returns></returns>
        public InputAction ResolveForPlayer(InputActionReference actionRef)
        {
            if (actionRef == null || actionRef.action == null)
                return null;
            if (PlayerIndex == -1)
                return actionRef.action;

            var m_cachedAction = actionRef.action;

            if (PlayerIndex != -1)
            {
                if (PlayerIndex < UnityEngine.InputSystem.Users.InputUser.all.Count)
                {
                    m_cachedAction = GetFirstMatch(UnityEngine.InputSystem.Users.InputUser.all[PlayerIndex], actionRef);
                }
                else
                {
                    m_cachedAction = null;
                }
            }


            // Update enabled status
            if (m_cachedAction != null && m_cachedAction.enabled != actionRef.action.enabled)
            {
                if (actionRef.action.enabled)
                    m_cachedAction.Enable();
                else
                    m_cachedAction.Disable();
            }

            return m_cachedAction;

            // local function to wrap the lambda which otherwise causes a tiny gc
            InputAction GetFirstMatch(in UnityEngine.InputSystem.Users.InputUser user, InputActionReference aRef) =>
                user.actions.First(x => x.id == aRef.action.id);
        }

        public Vector3 WorldToPlayerViewPosition(Vector3 worldPosition)
        {
            if (!_playerCam) return Vector3.zero;
            Vector3 toReturn = _playerCam.WorldToViewportPoint(worldPosition);
            toReturn.x *= _playerCam.pixelWidth;
            toReturn.y *= _playerCam.pixelHeight;

            return toReturn;
        }

        public Vector3 WorldToPlayerViewPositionNormalized(Vector3 worldPosition)
        {
            if (!_playerCam) return Vector3.zero;
            Vector3 toReturn = _playerCam.WorldToViewportPoint(worldPosition);

            return toReturn;
        }

        public static Player GetPlayerByID(int id)
        {
            foreach (var player in Players)
            {
                if (player.PlayerIndex == id)
                {
                    return player;
                }
            }

            return null;
        }

        public bool ConnectedWithPlayerInput()
        {
            return playerInput != null;
        }

        /// <summary>
        /// A single input setting
        /// Can be overridden to provide custom usage
        /// </summary>
        [Serializable]
        public class InputUnit
        {
            public enum InputUnitType
            {
                press,
                hold,
                release
            }
            
            [FormerlySerializedAs("_tag")] public GamePlayTag _inputTag = GamePlayTag.None;
            public InputActionReference _actionRef;
            protected InputAction _action;
            public InputUnitType type = InputUnitType.press;
            public float _inputBufferTime = 0.5f;
            
            protected float _bufferLife = 0;
            
            public bool Triggered()
            {
                if (_action == null) return false;//Ignore the check

                switch (type)
                {
                    case InputUnitType.press:
                        return _action.WasPressedThisFrame() || _bufferLife > 0;
                    case InputUnitType.hold:
                        return _action.phase == InputActionPhase.Performed;
                    case InputUnitType.release:
                        return _action.WasReleasedThisFrame();
                }
                
                return false;
            }

            void Initialize(Player player)
            {
                _actionRef.action.actionMap.Enable();
                _action = player.ResolveForPlayer(_actionRef);
            }


            public void OnTick(Player player)
            {
                if (_actionRef is null) return;
                if (_action == null) {Initialize(player); return;}

                if (_bufferLife > 0) _bufferLife -= Time.deltaTime;

                bool triggered = false;
                switch (type)
                {
                    case InputUnitType.press:
                        triggered = _action.WasPressedThisFrame();
                        break;
                    case InputUnitType.release:
                        triggered  =_action.WasReleasedThisFrame();
                        break;
                }
                if (triggered)
                {
                    _bufferLife = _inputBufferTime;
                }
            }

            public void ResetBuffer()
            {
                _bufferLife = 0;
            }

        }
    }
    
    
}
