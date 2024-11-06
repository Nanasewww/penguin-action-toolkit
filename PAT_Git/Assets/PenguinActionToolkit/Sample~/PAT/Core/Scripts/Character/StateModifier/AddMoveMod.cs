using UnityEngine;

namespace PAT
{
    public class AddMoveMod: StateModifier
    {
        public Vector3 move;
        public bool relative = true;

        public override void BeginEvent()
        {
            base.BeginEvent();

            Vector3 toAdd = move;
            if (relative)
            {
                toAdd = characterController.Locomotion.transform.TransformDirection(move);
            }
            characterController.Locomotion.AddMove(toAdd);
        }
    }
}