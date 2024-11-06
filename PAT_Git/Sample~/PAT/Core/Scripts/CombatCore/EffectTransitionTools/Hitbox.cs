using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PAT
{
    public class Hitbox : MonoBehaviour
    {
        public PATComponent owner;
        [SerializeField] protected bool _activated = false;
        protected readonly HashSet<Hurtbox> permantWhiteList = new HashSet<Hurtbox>();
        protected readonly HashSet<Hurtbox> tempWhiteList = new HashSet<Hurtbox>();
        [SerializeField] protected List<Effect> _effectList = new ();
        
        protected Vector3 lastPos;
        
        
        #region Getter
        public bool activated { get { return _activated; } }

        #endregion
        public void Activate()
        {
            if(!owner) owner = GetComponentInParent<PATComponent>();
            _activated = true;
            lastPos = transform.position;
            tempWhiteList.Clear();
        }

        public void DeActivate()
        {
            _activated = false;
            tempWhiteList.Clear();
        }

        public virtual void OnTriggerStay(Collider other)
        {
            TryHit(other);
        }

        protected virtual bool TryHit(Collider other)
        {
            if (!_activated)
            {
                return false;
            }

            Hurtbox receiveBox = other.GetComponent<Hurtbox>();

            if (!receiveBox) return false;
            if (permantWhiteList.Contains(receiveBox)) return false;
            if (tempWhiteList.Contains(receiveBox)) return false;
            if (receiveBox.owner.team == owner.team) return false;

            if (!receiveBox.CanBeHit()) return false;

            //Add the hitinfo
            Effect hitInfoEffect = BuildHitboxInfoEffect(other);
            _effectList.Add(hitInfoEffect);
            //The CORE
            owner.SendEffectPackage(receiveBox.owner, _effectList);
            //The CORE
            _effectList.Remove(hitInfoEffect);
            
            tempWhiteList.Add(receiveBox);
            return true;
        }

        Effect BuildHitboxInfoEffect(Collider other)
        {
            EffectHitboxInfo hitInfo = ScriptableObject.CreateInstance<EffectHitboxInfo>();
            hitInfo.hitbox = this;
            hitInfo.hitPosition = other.ClosestPoint(transform.position);
            hitInfo.hitRotation = Quaternion.LookRotation(hitInfo.hitPosition - other.transform.position);

            EffectLifeControl lifeControl = ScriptableObject.CreateInstance<EffectLifeControl>();
            lifeControl.removeOnApply = true;
            
            Effect effect = Effect.NewEffect(null,null,owner);
            effect.components.Add(hitInfo);
            effect.components.Add(lifeControl);
            
            return effect;
        }

        private void OnDisable()
        {
            ClearTempWhiteList();
        }
        
        public virtual void FixedUpdate()
        {
            if(!_activated) return;
            
            Vector3 dif = transform.position - lastPos;
            RaycastHit[] results = new RaycastHit[10];
            Physics.RaycastNonAlloc(transform.position, -dif.normalized, results, dif.magnitude);
            //Debug.DrawRay(transform.position, -dif.normalized, Color.blue);

            foreach (RaycastHit hit in results)
            {
                if(!hit.collider) continue;
                bool b = TryHit(hit.collider);
            }

            lastPos = transform.position;
        }

        public void UpdateEffectList(List<Effect> newEffects)
        {
            _effectList.Clear();
            foreach (var effect in newEffects)
            {
                _effectList.Add(effect);
            }
        }
        
        public void ClearTempWhiteList() { tempWhiteList.Clear(); }

    }
}
