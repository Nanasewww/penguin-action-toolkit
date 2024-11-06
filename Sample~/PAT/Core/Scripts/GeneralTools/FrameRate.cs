using UnityEngine;

namespace PAT
{
    public class FrameRate : MonoBehaviour
    {
        [SerializeField] protected int fps;


        private void Start()
        {
            Application.targetFrameRate = fps;
            Time.fixedDeltaTime = Mathf.Min(1.0f / fps, 1.0f / 60f);
        }   


    }
}
