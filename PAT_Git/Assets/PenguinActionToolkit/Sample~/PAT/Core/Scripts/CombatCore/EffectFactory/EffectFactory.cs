using System;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    [Serializable]
    public abstract class EffectFactory
    {
        public abstract List<Effect> GenerateEffect(PATComponent sourceComponent);
    }
}