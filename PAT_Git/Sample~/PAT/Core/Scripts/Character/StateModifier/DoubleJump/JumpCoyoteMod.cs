using UnityEngine;

namespace PAT
{
    public class JumpCoyoteMod: StateModifier
    {
        [SerializeField] protected float coyoteTime = 0.1f;
        private float ungroundedTime = 0;

        public override bool Validate(Character controller)
        {
            return !(!controller.Locomotion.onGround && CheckUngrounded()) && base.Validate(controller);
        }

        public void Update()
        {
            CheckUngrounded();
        }

        private bool CheckUngrounded()
        {
            if (characterController.Locomotion.onGround) { ungroundedTime = 0; }
            else { ungroundedTime += Time.deltaTime; }

            return ungroundedTime > coyoteTime;
        }
    }
}