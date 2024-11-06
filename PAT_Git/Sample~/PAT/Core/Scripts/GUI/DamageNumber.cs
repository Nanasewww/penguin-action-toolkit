using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class DamageNumber : MonoBehaviour
    {
        [SerializeField] protected int playerID = -1;
        [SerializeField] protected GameObject dmgNPrefab;
        [SerializeField] protected Vector3 offset;

        private Player player;
        private void Awake()
        {
            GameObject obj = Instantiate(dmgNPrefab);
            Destroy(obj);

            PATComponent.onAnyEffectPackageEvent += OnPackageEvent;
        }

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            player = Player.GetPlayerByID(playerID);
        }

        private void OnDestroy()
        {
            PATComponent.onAnyEffectPackageEvent -= OnPackageEvent;
        }

        public void OnPackageEvent(PATComponent.EffectPackage package)
        {
            if(player == null) {Initialize(); return; }
            
            //First we need to comfirm that there's damage involved
            List<EffectModValue> mods = Effect.GetComponentsFromList<EffectModValue>(package.effects);
            foreach (EffectModValue mod in mods)
            {
                if (mod.resourceTag == GamePlayTag.Health)
                {
                    SpawnDamageNumber(package, mod);
                }
            }
        }

        private void SpawnDamageNumber(PATComponent.EffectPackage package, EffectModValue mod)
        {
            //Then we spawn position accordingly, base one hitbox involved or not
            EffectHitboxInfo info = Effect.GetComponentFromList<EffectHitboxInfo>(package.effects);
            
            GameObject numObj = Instantiate(dmgNPrefab, transform);
            if (info != null)
            {
                //numObj.transform.position = offset + player.WorldToPlayerViewPosition(info.hitPosition);
                numObj.GetComponent<DamageNumberUnit>()?.Initialization(player, info.hitPosition, offset, mod);
            }
            else
            {
                //numObj.transform.position = offset + player.WorldToPlayerViewPosition(package.target.transform.position);
                numObj.GetComponent<DamageNumberUnit>()?.Initialization(player, package.target.transform.position, offset, mod);
            }
        }
    }
}
