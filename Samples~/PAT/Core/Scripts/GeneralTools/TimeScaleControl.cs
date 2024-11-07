using UnityEngine;

namespace PAT
{
    public class TimeScaleControl : MonoBehaviour
    {
        public float timeScale = 1;

        private void Update()
        {
            Time.timeScale = timeScale;
        }
    }
}


