using System;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    /// <summary>
    /// Very draft and risky method to apply effect
    /// Good for prototyping
    /// </summary>
    public class AddEffectBox: MonoBehaviour
    {
        public List<Effect> effects;

        private void OnTriggerEnter(Collider other)
        {
            Hurtbox box = other.GetComponent<Hurtbox>();
            if(!box) return;

            foreach (Effect effect in effects)
            {
                box.owner.AddEffect(Instantiate(effect));
            }
        }
    }
}