using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PAT
{
    public class OpenFeedbackWithHitbox: MonoBehaviour
    {
        public List<TrailRenderer> renderers = new List<TrailRenderer>();
        public AudioClip audioClip;
        public float volume = 1.0f;
        public Vector2 pitchRange = new Vector2(0.5f, 1.5f);
        public Hitbox hitbox;
        

        private bool lastEnabled = false;
        private void Awake()
        {
            if(hitbox == null) hitbox = GetComponent<Hitbox>();
        }

        private void Update()
        {
            foreach (TrailRenderer trailRenderer in renderers)
            {
                trailRenderer.emitting = hitbox.activated;
            }

            if (!lastEnabled && hitbox.activated)
            {
                if (audioClip != null)
                {
                    AudioSource audioSource = new GameObject
                    {

                    }.AddComponent<AudioSource>();
                    audioSource.pitch *= Random.Range(pitchRange.x, pitchRange.y);
                    audioSource.volume = volume;
                    audioSource.PlayOneShot(audioClip);
                    StartCoroutine(DestroyAudio(audioSource));
                }
            }
            
            lastEnabled = hitbox.activated;
        }
        
        IEnumerator DestroyAudio(AudioSource audioSource)
        {
            yield return new WaitForSeconds(audioClip.length);
            Destroy(audioSource.gameObject);
        }
    }
}