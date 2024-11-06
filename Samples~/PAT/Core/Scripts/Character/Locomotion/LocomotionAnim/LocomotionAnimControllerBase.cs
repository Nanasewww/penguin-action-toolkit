using UnityEngine;

namespace PAT
{
    public interface LocomotionAnimControllerBase
    {
        /// <summary>
        /// This is called to initialize the animation controller.
        /// </summary>
        /// <param name="locomotion">where this controller is attached to</param>
        public void Initialize(CharacterLocomotionBase locomotion);
        
        /// <summary>
        /// This is called in FixedUpdate() to update locomotion-related animation parameters and states.
        /// </summary>
        /// <param name="locomotion">where this controller is attached to</param>
        public void MovementAnimation(CharacterLocomotionBase locomotion);
    }
}