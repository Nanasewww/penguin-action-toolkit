using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PAT
{
    public enum RemovalType
    {
        effectsWithAllTags,
        effectsWithAnyTags
    }


    public class RemoveEffectMod : StateModifier
    {
        [SerializeField] protected RemovalType tagBaseRemovalType;
        [SerializeField] protected List<GamePlayTag> removeTargetTags;
        public override void OnEnter(Character controller)
        {
            base.OnEnter(controller);

            if (tagBaseRemovalType == RemovalType.effectsWithAllTags)
            {
                for (int i = controller.effects.Count - 1; i >= 0; i--)
                {
                    bool result = true;
                    foreach (GamePlayTag tag in removeTargetTags)
                    {
                        if (!controller.effects[i].HasTag(tag))
                        {
                            result = false;
                            break;
                        }
                    }
                    if (result) { controller.RemoveEffect(controller.effects[i]); }
                }
            }

            if (tagBaseRemovalType == RemovalType.effectsWithAnyTags)
            {
                for (int i = controller.effects.Count - 1; i >= 0; i--)
                {
                    bool result = false;
                    foreach (GamePlayTag tag in removeTargetTags)
                    {
                        if (controller.effects[i].HasTag(tag))
                        {
                            result = true;
                            break;
                        }
                    }
                    if (result) { controller.RemoveEffect(controller.effects[i]); }
                }
            }
        }
    }
}
