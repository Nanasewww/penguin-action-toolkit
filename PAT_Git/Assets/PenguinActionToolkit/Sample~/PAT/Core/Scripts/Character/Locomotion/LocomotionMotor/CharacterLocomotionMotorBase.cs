using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public interface CharacterLocomotionMotorBase
    {
        /// <summary>
        /// This is called to apply current pending movement to the character.
        /// </summary>
        /// <remarks>The pending movement can be accessed by using locomotion.currentMovement. 
        /// Normally you want to use locomotion.characterTransform as your target in case the
        /// locomotion component is not attached to character's transform</remarks>
        /// <param name="locomotion">where this motor is attached to</param>
        void ApplyMovement(CharacterLocomotionBase locomotion);
        
        /// <summary>
        /// This is called to rotate the character toward current pending direction.
        /// </summary>
        /// <remarks>The pending direction can be accessed by using locomotion.currentRotationDirection.
        /// Normally you want this to be controlled by rotation speed as well.</remarks>>
        /// <param name="locomotion">where this motor is attached to</param>
        void ApplyRotation(CharacterLocomotionBase locomotion);
        
        /// <summary>
        /// This is called to check whether the character is on the ground.
        /// </summary>
        /// <param name="locomotion">where this motor is attached to</param>
        /// <returns>Whether the character is on the ground</returns>
        bool CheckIfGrounded(CharacterLocomotionBase locomotion);
        
        /// <summary>
        /// This is called to teleport the character to a target position.
        /// </summary>
        /// <param name="locomotion">where this motor is attached to</param>
        /// <param name="destPosition">teleport destination</param>
        void MoveToPosition(CharacterLocomotionBase locomotion, Vector3 destPosition);
        
        /// <summary>
        /// This is called to instantly change the rotation of character.
        /// </summary>
        /// <remarks>This version uses float and can only control Y axis. </remarks>
        /// <param name="locomotion">where this motor is attached to</param>
        /// <param name="angle">instant rotating angle</param>
        void InstantRotate(CharacterLocomotionBase locomotion, float angle);
    }
}
