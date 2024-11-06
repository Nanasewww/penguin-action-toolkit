using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace PAT
{
    public class Projectile: Hitbox
    {
        [SerializeField] protected float _speed;
        [SerializeField] protected Vector3 _dir;
        [SerializeField] protected int _hitNum = 1;
        [SerializeField] protected float _life = 10f;

        [SerializeReference, SubclassPicker] private EffectFactory dmgFactory;
  
        public UnityEvent onDestroy;

        protected int hp;

        #region Getter
        public float speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public Vector3 dir
        {
            get { return _dir; }
            set { _dir = value; }
        }
        #endregion

        private void Awake()
        {
            hp = _hitNum;
            StartCoroutine(End());
        }
        
        protected override bool TryHit(Collider c)
        {
            UpdateEffectList(dmgFactory.GenerateEffect(owner));
            bool hitted = base.TryHit(c);

            if (!hitted) return false;

            if (_hitNum > 0) hp --;
            if (hp <= 0) {onDestroy.Invoke();Destroy(gameObject);}
            return hitted;
        }
        
        public override void FixedUpdate()
        {
            transform.Translate(_speed * _dir.normalized * Time.fixedDeltaTime, Space.World);
            float dis = (_speed * _dir.normalized * Time.fixedDeltaTime).magnitude;
            
            // RaycastHit[] hits = Physics.RaycastAll(transform.position, -_dir.normalized, dis);
            // //Debug.DrawRay(transform.position, -_dir.normalized, Color.blue);
            // foreach (RaycastHit hit in hits)
            // {
            //     bool b =TryHit(hit.collider);
            //     if(hp <= 0)return;
            // }
            base.FixedUpdate();
        }

        IEnumerator End()
        {
            yield return new WaitForSeconds(_life);
            onDestroy.Invoke();
            Destroy(gameObject);
        }
    }
}