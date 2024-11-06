using System;
using UnityEngine;

namespace PAT
{
    public class CharacterSwapMode: MonoBehaviour
    {
        private void Awake()
        {
            PATComponent.onAnyEffectPackageEvent += AttemptChangeCharacter;
        }

        private void OnDestroy()
        {
            PATComponent.onAnyEffectPackageEvent -= AttemptChangeCharacter;
        }

        void AttemptChangeCharacter(PATComponent.EffectPackage info)
        {
            if (info.source == Player.Players[0].character
                && info.source is Character)
            {
                Player.Players[0].ChangeCharacter((Character)info.target);
                if (info.source.GetComponentInChildren<AiBrain>())
                {
                    info.source.GetComponentInChildren<AiBrain>().enabled = true;
                }
                if (info.target.GetComponentInChildren<AiBrain>())
                {
                    info.target.GetComponentInChildren<AiBrain>().enabled = false;
                }
            }
        }
    }
}