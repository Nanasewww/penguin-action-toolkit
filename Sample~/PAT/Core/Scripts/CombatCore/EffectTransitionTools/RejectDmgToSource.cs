using System;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class RejectDmgToSource: MonoBehaviour
    {
        [SerializeField] private PATComponent owner;
        [SerializeReference, SubclassPicker] private EffectFactory dmgFactory;

        private void Awake()
        {
            owner = gameObject.GetComponentInParent<PATComponent>();
        }
        
        public void RejectDmg(PATComponent.EffectPackage package)
        {
            if(!package.source) return;
            List<Effect> effects = dmgFactory.GenerateEffect(owner);
            
            owner.SendEffectPackage(package.source, effects);
        }
    }
}