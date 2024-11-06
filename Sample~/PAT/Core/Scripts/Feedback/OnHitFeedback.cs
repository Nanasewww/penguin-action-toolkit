using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class OnHitFeedback : MonoBehaviour
    {
        public PATComponent owner;
        public Hitbox referenceHitbox;
        public ParticleSystem particle;
        public AudioClip audioClip;
        public float volume = 1.0f;
        public float scale = 10f;
        
        protected float damageScale = 1;

        private void Awake()
        {
            if (!owner) owner = GetComponentInParent<PATComponent>();
            if (!referenceHitbox) referenceHitbox = GetComponent<Hitbox>();
            owner.onEffectPackageSent += OnHit;
        }

        private void OnDestroy()
        {
            owner.onEffectPackageSent -= OnHit;
        }

        void OnHit(PATComponent.EffectPackage package)
        {
            EffectHitboxInfo info = Effect.GetComponentFromList<EffectHitboxInfo>(package.effects);
            EffectModValue modValue = Effect.GetComponentFromList<EffectModValue>(package.effects);
            
            if (info== null) return;
            if (modValue == null) return;
            if (referenceHitbox && info.hitbox != referenceHitbox) return;

            damageScale = (-modValue.value) / scale;

            if (particle != null)
            {
                ParticleSystem toPlay = Instantiate(particle, referenceHitbox.transform.position, Quaternion.identity);
                toPlay.transform.position = info.hitPosition;
                toPlay.transform.rotation = info.hitRotation;
                toPlay.transform.localScale *= damageScale;
                toPlay.Play();
                
            }

            if (audioClip != null)
            {
                AudioSource audioSource = new GameObject
                {
                    transform =
                    {
                        position = info.hitPosition,
                        rotation = info.hitRotation
                    }
                }.AddComponent<AudioSource>();
                audioSource.pitch *= damageScale;
                audioSource.volume = volume;
                audioSource.PlayOneShot(audioClip);
                StartCoroutine(DestroyAudio(audioSource));
            }
        }

        IEnumerator DestroyAudio(AudioSource audioSource)
        {
            yield return new WaitForSeconds(audioClip.length);
            Destroy(audioSource.gameObject);
        }
        
        
    }
}