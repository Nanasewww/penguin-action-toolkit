using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    [Serializable]
    public class LocomotionData
    {
        [SerializeField][FormerlySerializedAs("MoveSpeed")]private float _moveSpeed;
        [SerializeField][FormerlySerializedAs("rotationSpeed")]private float _rotationSpeed;
        [SerializeField][FormerlySerializedAs("locomotionAnimationSpeed")]private float _locomotionAnimationSpeed;
        [SerializeField][FormerlySerializedAs("rootMotionMutiplier")]private float _rootMotionMutiplier;
        
        public float MoveSpeed {get {return _moveSpeed;} set{_moveSpeed = value;}}
        public float rotationSpeed {get {return _rotationSpeed;} set{_rotationSpeed = value;}}
        public float locomotionAnimationSpeed {get {return _locomotionAnimationSpeed;} set{_locomotionAnimationSpeed = value;}}
        public float rootMotionMutiplier {get {return _rootMotionMutiplier;} set{_rootMotionMutiplier = value;}}
        public bool ignoreGravity = false;
    }

    public class LocomotionAttribute
    {
        public float MoveSpeed {get {return  moveSpeedAttribute.currentAmount;} set{moveSpeedAttribute.SetBaseValue(value);}}
        public float rotationSpeed {get {return rotationSpeedAttribute.currentAmount;} set{rotationSpeedAttribute.SetBaseValue(value);}}
        public float locomotionAnimationSpeed {get {return locoAniSpeedAttribute.currentAmount;} set{locoAniSpeedAttribute.SetBaseValue(value);}}
        public float rootMotionMutiplier {get {return rootMotionMutiAttribute.currentAmount;} set{rootMotionMutiAttribute.SetBaseValue(value);}}
        public bool ignoreGravity = false;//todo: do we really want this? Do we want to change this to gravity value?
        
        private Attribute moveSpeedAttribute;
        private Attribute rotationSpeedAttribute;
        private Attribute locoAniSpeedAttribute;
        private Attribute rootMotionMutiAttribute;
        public void Initialize(GameObject attributeHolder, LocomotionData data)
        {
            moveSpeedAttribute = attributeHolder.AddComponent<Attribute>();
            rotationSpeedAttribute = attributeHolder.AddComponent<Attribute>();
            locoAniSpeedAttribute = attributeHolder.AddComponent<Attribute>();
            rootMotionMutiAttribute = attributeHolder.AddComponent<Attribute>();

            moveSpeedAttribute.resourceTag = GamePlayTag.locoSpeed;
            rotationSpeedAttribute.resourceTag = GamePlayTag.locoRotationSpeed;
            locoAniSpeedAttribute.resourceTag = GamePlayTag.locoAnimSpeed;
            rootMotionMutiAttribute.resourceTag = GamePlayTag.locoRootMulti;
            
            moveSpeedAttribute.maxAmount = 9999;
            rotationSpeedAttribute.maxAmount = 9999;
            locoAniSpeedAttribute.maxAmount = 9999;
            rootMotionMutiAttribute.maxAmount = 9999;
            
            moveSpeedAttribute.SetBaseValue(data.MoveSpeed);
            rotationSpeedAttribute.SetBaseValue(data.rotationSpeed);
            locoAniSpeedAttribute.SetBaseValue(data.locomotionAnimationSpeed);
            rootMotionMutiAttribute.SetBaseValue(data.rootMotionMutiplier);
        } 
    }

    /// <summary>
    /// Those data are not likely to be modified a lot
    /// Thus I keep them here
    /// </summary>
    [Serializable]
    public class LocomotionExtraData
    {
        public float groundAcc = 50f;
        public float groundDamp = 25f;
        public float airSpeedRatio = 0.8f;
        public float airAcc = 25f;
        public float airDamp = 0f;
    }
    
    [Serializable]
    public class Locomotion3DForce
    {
        public Vector3 dir;
        public float magnitude = 2 ;
        public float decaySpeed = 1;
    }



    public class CharacterLocomotionBase : MonoBehaviour
    {
        [Header("Motor")]
        [SerializeField] protected CharacterLocomotionMotorBase _locomotionMotor;
        [SerializeField] protected LocomotionAnimControllerBase LocomotionAnimController;
        
        [Header("Character Reference")]
        [SerializeField] protected Character _character;
        [SerializeField] protected Transform _characterTransform;
        
        [Header("Attributes")]
        public Vector3 Gravity = new Vector3(0, -30f, 0);
        [FormerlySerializedAs("baseData")] [SerializeField] protected LocomotionData _baseData;
        protected readonly LocomotionAttribute _currentAttribute = new LocomotionAttribute();
        [FormerlySerializedAs("baseExtraData")] [SerializeField] protected LocomotionExtraData _baseExtraData;
        protected LocomotionExtraData _currentExtraData;
        
        [Header("Runtime Inspector")]
        [FormerlySerializedAs("onGround")] [SerializeField] protected bool _onGround;
        [Tooltip("The desired movement direction, clear at every update")][SerializeField] protected Vector3 _currentMoveDirection;
        [Tooltip("The desired rotation direction, clear at every update")][SerializeField] protected Vector3 _currentRotateDirection;
        [FormerlySerializedAs("currentMovement")] [SerializeField] protected Vector3 _currentMovement = Vector3.zero;//character direct controlled movements, influenced by damp
        [FormerlySerializedAs("extraMovement")] [SerializeField] protected Vector3 _extraMovement = Vector3.zero;//forces, and others, wouldn't be limited, clear on every update
        protected List<Locomotion3DForce> _forces = new List<Locomotion3DForce>();

        bool _moveSetted = false;
        bool _rotateSetted = false;
        
        #region Getter

        public bool onGround { get { return _onGround; } }
        public LocomotionAttribute currentAttribute { get { return _currentAttribute; } }
        public LocomotionExtraData currentExtraData { get { return _currentExtraData; } }
        public Vector3 currentMoveDirection { get { return _currentMoveDirection; } }
        public Vector3 currentRotateDirection { get { return _currentRotateDirection; } }
        public Vector3 currentMovement { get { return _currentMovement; } }
        public Vector3 extraMovement { get { return _extraMovement; } }
        public Character character { get { return _character; } }
        public Transform characterTransform { get { return _characterTransform; } }

        #endregion

        public virtual void Reset()
        {
            _baseData = new LocomotionData
            {
                MoveSpeed = 5f,
                rotationSpeed = 720f,
                locomotionAnimationSpeed = 1f,
                rootMotionMutiplier = 1f
            };
        }

        public void Awake()
        {
            _currentAttribute.Initialize(gameObject, _baseData);
            
            _currentExtraData = _baseExtraData;
            
            if (_character == null) { _character = GetComponent<Character>(); }
            if (_character == null) { _character = GetComponentInParent<Character>(); }
            if (_characterTransform == null) { _characterTransform = _character.transform; }
            if (_locomotionMotor == null) { _locomotionMotor = GetComponentInChildren<CharacterLocomotionMotorBase>(); }
        }

        public virtual void Start()
        {
            if (LocomotionAnimController == null)
            {
                // TODO: there might not be animations
                LocomotionAnimController = gameObject.AddComponent<LocomotionAnimController3D>();
            }
            LocomotionAnimController.Initialize(this);
        }


        // Update is called once per frame
        public virtual void FixedUpdate()
        {
            // Process
            _onGround = _locomotionMotor.CheckIfGrounded(this);
            ProcessPlaneMovement();
            if (LocomotionAnimController != null) LocomotionAnimController.MovementAnimation(this);
            ProcessForce();
            ProcessVerticalMovement();
            ProcessOtherMovement();
            
            // Apply (using motor)
            _locomotionMotor.ApplyMovement(this);
            if(_currentRotateDirection != Vector3.zero)_locomotionMotor.ApplyRotation(this);
            
            ProcessPlaneDamp();
            _extraMovement = Vector3.zero;
        }


        private void Update()
        {
            if(_moveSetted) _moveSetted = false;
            else _currentMoveDirection = Vector3.zero;
            if(_rotateSetted) _rotateSetted = false;
            else _currentRotateDirection = Vector3.zero;
        }
        
        /// <summary>
        /// Call this every update to control move direction
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetCurrentMoveDirection(Vector3 value)
        {
            _currentMoveDirection = value;
            _moveSetted = true;
        }

        public virtual float FixedDeltaTime() { return character.fixedDeltaTime; }
        
        /// <summary>
        /// Call this every update to control rotation direction
        /// </summary>
        /// <param name="dir"></param>
        public virtual void SetCurrentRotateDirection(Vector3 dir)
        {
            _currentRotateDirection = dir;
            _rotateSetted = true;
        }

        public virtual void ProcessPlaneMovement()
        {
            //Determine the data pick
            float maxSpeed = _currentAttribute.MoveSpeed;
            if (!_onGround) maxSpeed =  _currentAttribute.MoveSpeed * _currentExtraData.airSpeedRatio;
            //Speed is influence by input, while acc shall not
            maxSpeed *= new Vector3(_currentMoveDirection.x, 0, _currentMoveDirection.z).magnitude; 
            
            float acc = _currentExtraData.groundAcc;
            if (!_onGround) acc = _currentExtraData.airAcc;
                
            //Acc if did not reach maximum speed
            Vector3 planeMove = _currentMovement; planeMove.y = 0;
            Vector3 newMovement = planeMove + new Vector3(_currentMoveDirection.x, 0, _currentMoveDirection.z).normalized * acc * FixedDeltaTime();
            
            //Case not max yet && slowing down
            if (newMovement.magnitude <= maxSpeed || newMovement.magnitude < planeMove.magnitude)
            {
                _currentMovement.x = newMovement.x;
                _currentMovement.z = newMovement.z;
            }

            //Case exact max speed
            else if (newMovement.magnitude > maxSpeed && planeMove.magnitude < maxSpeed)
            {
                _currentMovement.x = (newMovement.normalized * maxSpeed).x;
                _currentMovement.z = (newMovement.normalized * maxSpeed).z;
            }
        }

        public virtual void ProcessPlaneDamp()
        {
            //Determine the data pick
            float damp = _currentExtraData.groundDamp;
            if (!_onGround) damp = _currentExtraData.airDamp;

            Vector3 planeMove = _currentMovement; planeMove.y = 0;
            
            Vector3 oldNormalize = planeMove.normalized;
            _currentMovement -= oldNormalize * damp * FixedDeltaTime();
            
            //avoid over damp
            planeMove = _currentMovement;planeMove.y = 0;
            if (planeMove.normalized != oldNormalize)
            {
                _currentMovement.x = 0;
                _currentMovement.z = 0;
            }
        }

        public virtual void ProcessVerticalMovement()
        {
            if (!_onGround && ! _currentAttribute.ignoreGravity)
            {
                _currentMovement += Gravity * FixedDeltaTime();
            }
            else
            {
                if ( _currentAttribute.ignoreGravity)
                {
                    float damp = _currentExtraData.groundDamp;
                    float oldSign = Mathf.Sign(_currentMovement.y);
                    _currentMovement -= damp * FixedDeltaTime() * new Vector3(0, _currentMovement.normalized.y, 0) ;
            
                    //avoid over damp
                    if (Mathf.Sign(_currentMovement.y) != oldSign)
                    {
                        _currentMovement.y = 0;
                    }
                }

                if (_onGround)
                {
                    _currentMovement.y = Mathf.Max(_currentMovement.y, 0);
                }
            }
        }

        public virtual void ProcessForce()
        {
            for (int i = _forces.Count - 1; i >= 0; i--)
            {
                _extraMovement += _forces[i].dir.normalized * _forces[i].magnitude;

                _forces[i].magnitude -= _forces[i].decaySpeed * FixedDeltaTime();
                if(_forces[i].magnitude <= 0) _forces.RemoveAt(i);
            } 
        }
        
        /// <summary>
        /// Write your own custom movement code
        /// Flying, graivty, diving etc
        /// </summary>
        public virtual void ProcessOtherMovement()
        {
            //Meant to be virtual
        }

        #region Helper
        
        /// <summary>
        /// Movements that stays, influenced by damping
        /// </summary>
        /// <param name="addedMove"></param>
        public virtual void AddMove(Vector3 addedMove)
        {
            _currentMovement += addedMove;
        }
        
        
        /// <summary>
        /// Never Call this in Update, instead use FIxedUpdate
        /// Movements that clear on every update
        /// </summary>
        /// <param name="addMove"></param>
        public virtual void AddExtraMove(Vector3 addMove)
        {
            _extraMovement += addMove;
        }

        public virtual void MoveToPosition(Vector3 position)
        {
            _locomotionMotor.MoveToPosition(this, position);
        }
        

        /// <summary>
        /// For other script to control facing directly
        /// todo: are we sure we only want y axis rotation?
        /// </summary>
        /// <param name="angle"></param>
        public void InstantRotate(float angle)
        {
            _locomotionMotor.InstantRotate(this, angle);
        }
        
        /// <summary>
        /// Return the input direction based on character
        /// </summary>
        /// <returns></returns>
        public virtual Vector3 GetCharacterBasedInputDirection()
        {
            return _characterTransform.InverseTransformDirection(_currentMoveDirection);
        }
        

        public virtual void SetCurrentInputDirection(Vector3 value)
        {
            _character.inputDirection = value;
        }

        public virtual void SetDirectionBaseOnFacing(Vector3 value)
        {
            SetCurrentMoveDirection(_characterTransform.TransformDirection(value));
        }

        public virtual void AddForce(Locomotion3DForce f)
        {
            Locomotion3DForce force = new Locomotion3DForce
            {
                magnitude = f.magnitude,
                dir = f.dir,
                decaySpeed = f.decaySpeed
            };
            _forces.Add(force);
        }
        #endregion


    }
}
