using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class TrailControlMod : StateModifier
    {
        [SerializeField] protected List<TrailRenderer> renderers = new List<TrailRenderer>();

        public override void BeginEvent()
        {
            base.BeginEvent();

            foreach (var trailRenderer in renderers) { trailRenderer.emitting = true; }
        }

        public override void EndEvent()
        {
            base.EndEvent();

            foreach (var trailRenderer in renderers) { trailRenderer.emitting = false; }
        }
    }
}
