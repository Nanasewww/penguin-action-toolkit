using System;
using UnityEngine;

namespace PAT
{
    public class AnimationHelper: MonoBehaviour
    {
        public Animator animator;

        private void Awake()
        {
            if (!animator) animator = GetComponent<Animator>();
        }

        public void ActivateBool(string t)
        {
            if(!animator) return;
            animator.SetBool(t, true);
        }

        public void DeactivateBool(string t)
        {
            if (!animator) return;
            animator.SetBool(t, false);
        }
    }
}