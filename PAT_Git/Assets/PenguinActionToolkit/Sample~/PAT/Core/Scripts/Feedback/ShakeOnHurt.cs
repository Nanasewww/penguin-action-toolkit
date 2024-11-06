using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class ShakeOnHurt: MonoBehaviour
    {
        public Transform toShake;
        public PATComponent owner;
        public GamePlayTag hurtTag = GamePlayTag.Health;

        public float shakeDuration = 0.5f; // Duration of the shake effect
        public float shakeIntensity = 0.5f; // Intensity of the shake effect
        public float shakeFrequency = 20f; // Frequency of the shake effect (higher means faster shaking)

        private Vector3 originalPosition;
        private Coroutine shakeCoroutine;
        
        private void Awake()
        {
            if(!toShake) toShake = transform;
            if (!owner) owner = GetComponentInParent<PATComponent>();
            if (!owner)
            {
                Debug.LogWarning("No owner assigned, wouldn't be working");
                return;
            }

            owner.onEffectPackageReceived += CheckIfShake;
        }

        private void OnDestroy()
        {
            if(owner) owner.onEffectPackageReceived -= CheckIfShake;
        }

        void CheckIfShake(PATComponent.EffectPackage package)
        {
            List<EffectModValue> mods = Effect.GetComponentsFromList<EffectModValue>(package.effects);
            foreach (var mod in mods)
            {
                if(mod.resourceTag == hurtTag && mod.value <= 0){StartShake(); return;}
            }
        }

        public void StartShake()
        {
            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
                toShake.localPosition = originalPosition;
            }
            shakeCoroutine = StartCoroutine(Shake());
        }
        
        private IEnumerator Shake()
        {
            float elapsed = 0f;
            originalPosition = toShake.localPosition;

            while (elapsed < shakeDuration)
            {
                float xOffset = Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) * 2 - 1;
                float yOffset = Mathf.PerlinNoise(0f, Time.time * shakeFrequency) * 2 - 1;

                toShake.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0) * shakeIntensity;

                elapsed += Time.deltaTime;
                yield return null;
            }

            toShake.localPosition = originalPosition; // Reset to original position after shake
        }
    }
    
    
}