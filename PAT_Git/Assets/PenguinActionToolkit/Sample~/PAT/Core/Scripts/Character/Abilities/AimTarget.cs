using System;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class AimTarget : MonoBehaviour
    {
        public static List<AimTarget> LockAbleList;
        public Character character;

        public event Action onRemoveFromList;
        


        private void Awake()
        {
            if (LockAbleList == null) LockAbleList = new List<AimTarget>();

            LockAbleList.Add(this);

            if (!character) character = GetComponentInParent<Character>();
        }

        private void Start()
        {
            
            if (character) character.healthAttribute.OnBaseValueReachMin += (() => {Destroy(this); });
        }

        private void OnDisable()
        {
            LockAbleList.Remove(this);
            onRemoveFromList?.Invoke();
        }

        private void OnEnable()
        {
            LockAbleList.Add(this);
        }

        private void OnDestroy()
        {
            LockAbleList.Remove(this);
            onRemoveFromList?.Invoke();
        }

        public float CameraDistance(Camera cam)
        {
            Vector3 result = cam.WorldToViewportPoint(transform.position);
            return result.z;
        }
        
        public Vector3 CameraPosition(Camera cam)
        {
            if (!cam) {Debug.LogWarning("No Cam Pass"); return Vector3.zero; }
            var result = cam.WorldToViewportPoint(transform.position);

            return result;
        }

        /// <summary>
        /// Returns the team of this lockable's character
        /// If no character assigned, returns 0
        /// </summary>
        /// <returns></returns>
        public int GetTeam()
        {
            if (character) return character.team;
            return 0;
        }

        
    }
}
 