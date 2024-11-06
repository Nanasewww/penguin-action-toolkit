using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PAT
{
    //todo: make this an example of combat feedback
    public class PlayParticleOnHit: MonoBehaviour
    {
        public PATComponent owner;
        public Hitbox referenceHitbox;
        public ParticleSystem particle;
        public float minScale = 0.3f;

        protected Vector3 o_scale;

        private void Awake()
        {
            if (!owner) owner = GetComponentInParent<PATComponent>();
            o_scale = particle.transform.localScale;
            owner.onEffectPackageSent += OnHit;
        }

        private void OnDestroy()
        {
            owner.onEffectPackageSent -= OnHit;
        }

        void OnHit(PATComponent.EffectPackage package)
        {
            ParticleSystem toPlay = particle;
            
            if(!toPlay)return;

            EffectHitboxInfo info = Effect.GetComponentFromList<EffectHitboxInfo>(package.effects);
            
            if(info == null)return;
            if(referenceHitbox && info.hitbox != referenceHitbox) return;   

            toPlay.transform.position = info.hitPosition;
            toPlay.transform.rotation = info.hitRotation;
            toPlay .Play();
        }
    }
}