using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PAT
{
    public class DisableOnStart : MonoBehaviour
    {
        void Start()
        {
            gameObject.SetActive(false);
        }

    }
}
