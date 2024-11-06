using UnityEngine;

namespace PAT
{
    public class ConstantMoveMod: StateModifier
    {
        public Vector3 move;
        public bool relative = true;
        
        private bool moving = false;

        public override void BeginEvent()
        {
            base.BeginEvent();
            moving = true;
        }

        public override void EndEvent()
        {
            base.EndEvent();
            moving = false;
        }

        public override void EarlyOnUpdate(Character controller)
        {
            base.EarlyOnUpdate(controller);

            if(!moving) return;
            
            Vector3 newMove = move;
            if(relative) newMove = controller.Locomotion.transform.TransformDirection(newMove);
            
            controller.Locomotion.AddExtraMove(newMove);
        }
    }
}