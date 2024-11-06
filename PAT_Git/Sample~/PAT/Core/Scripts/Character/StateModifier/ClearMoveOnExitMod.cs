using UnityEngine;

namespace PAT
{
    public class ClearMoveOnExitMod : StateModifier
    {
        public override void OnExit(Character controller)
        {
            base.OnExit(controller);
            controller.Locomotion.AddMove(-controller.Locomotion.currentMovement);
        }
    }
}