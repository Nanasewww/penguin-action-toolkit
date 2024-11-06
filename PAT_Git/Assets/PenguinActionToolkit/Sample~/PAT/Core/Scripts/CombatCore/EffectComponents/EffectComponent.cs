using UnityEngine;

namespace PAT
{
    /// <summary>
    /// I made this a scriptable object to serilize it
    /// There's no need to actually create assets for any effect component in my opinion
    /// </summary>
    [System.Serializable]
    public abstract class EffectComponent: ScriptableObject
    {
        public abstract void OnApply(Effect effect);
        
        public abstract void OnTick(Effect effect);
        
        public abstract void OnRemove(Effect effect);
    }
}