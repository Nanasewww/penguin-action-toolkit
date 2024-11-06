using System.Numerics;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace PAT
{
    /// <summary>
    /// This is a data container for hitbox
    /// Hitbox will input some important information in 
    /// </summary>
    public class EffectHitboxInfo: EffectComponent
    {
        public Hitbox hitbox;
        public Vector3 hitPosition;
        public Quaternion hitRotation;
        public override void OnApply(Effect effect)
        {
            
        }

        public override void OnTick(Effect effect)
        {
            
        }

        public override void OnRemove(Effect effect)
        {
            
        }
    }
}