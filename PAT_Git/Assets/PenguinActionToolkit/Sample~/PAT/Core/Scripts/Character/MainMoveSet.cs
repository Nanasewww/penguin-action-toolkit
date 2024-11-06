using UnityEngine;

namespace PAT
{
    public class MainMoveSet: MoveSet
    {
        [Header("Core States")]
        [SerializeField] protected ActionState _idleState;
        [SerializeField] protected ActionState _deathState;
        //[SerializeField] protected ActionState _hurtState;
        
        public ActionState idleState { get { return _idleState; } }
        public ActionState deathState{ get { return _deathState; } }
        //public ActionState hurtState { get { return _hurtState; } }
    }
    
}