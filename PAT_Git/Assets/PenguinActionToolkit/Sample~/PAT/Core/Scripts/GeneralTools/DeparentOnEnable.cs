using System;
using UnityEngine;

namespace PAT
{
    public class DeparentOnEnable: MonoBehaviour
    {
        private void Awake()
        {
            transform.parent = null;
        }

        private void OnEnable()
        {
            transform.parent = null;
        }
    }
}