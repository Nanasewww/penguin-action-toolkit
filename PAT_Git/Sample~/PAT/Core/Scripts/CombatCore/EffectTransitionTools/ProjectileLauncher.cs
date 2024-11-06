using System;
using UnityEngine;
using UnityEngine.Events;

namespace PAT
{
    public class ProjectileLauncher: MonoBehaviour
    {
        public Character _owner;
        public GameObject projectile;
        public UnityEvent onFire;
        
        private void Awake()
        {
            if(!_owner) _owner = GetComponentInParent<Character>();
        }

        public virtual void FireProjectile()
        {
            
            Projectile p = Instantiate(projectile, transform.position, transform.rotation, null)
                .GetComponent<Projectile>();

            if (!p) return;

            p.owner = _owner;
            p.dir = p.transform.TransformDirection(Vector3.forward);
            
            onFire.Invoke();
        }
    }
}