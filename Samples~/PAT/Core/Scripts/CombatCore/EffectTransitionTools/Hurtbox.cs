using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class Hurtbox : MonoBehaviour
    {
        [SerializeField] protected PATComponent _owner;
        [SerializeField] protected GamePlayTag _iFrameTag = GamePlayTag.Invincible;
        
        public PATComponent owner { get { return _owner; } }
        void Start()
        {
            if (!_owner) _owner = GetComponentInParent<PATComponent>();
        }

        public virtual bool CanBeHit()
        {
            return !owner.tagContainer.CheckForTag(_iFrameTag);
        }
        
        
    }
}
