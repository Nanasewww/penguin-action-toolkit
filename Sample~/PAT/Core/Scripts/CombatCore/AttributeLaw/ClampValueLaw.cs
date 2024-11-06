using UnityEngine;


namespace PAT
{
    [CreateAssetMenu(menuName = "PAT/AttributeLaw/ClampValueLaw")]
    public class ClampValueLaw : AttributeLaw
    {
        public float min;
        public float max;
        public bool roundToInt = false;

        public override EffectModValue ApplyLawToMod(Attribute attribute, EffectModValue mod)
        {
            mod.value = Mathf.Clamp(mod.value, min, max);
            if (roundToInt)
            {
                mod.value = Mathf.RoundToInt(mod.value);
            }

            return mod;
        }
    }
}
