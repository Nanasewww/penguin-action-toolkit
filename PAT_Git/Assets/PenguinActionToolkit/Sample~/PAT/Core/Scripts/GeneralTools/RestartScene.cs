using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace PAT
{
    public class RestartScene : MonoBehaviour
    {
        public InputActionReference input;
        private void Update()
        {
            if (input.action.WasPressedThisFrame())
            {
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            }
        }
    }
}
