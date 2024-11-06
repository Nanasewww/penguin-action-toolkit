using System;
using UnityEngine;
using System.Collections.Generic;

namespace PAT
{

    public class Health_PAT : Attribute
    {
        protected override void EffectOneTimeMod(EffectModValue mod)
        {

            /*Attribute resist = owner.GetAttributeByTag(GamePlayTag.Resistance);
            Attribute reduce = owner.GetAttributeByTag(GamePlayTag.damageReduce);
            
            //If a successful block happens
            if (mod.level < resist.currentAmount)
            {
                mod.value *= 1 - reduce.currentAmount;
                mod.ownerEffect.mainTags.Add(GamePlayTag.Block);
            }*/
            
            //We process damage after reducing damage
            base.EffectOneTimeMod(mod);
        }
        
        
    }

}