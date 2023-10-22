using UnityEngine;

namespace _Scripts.Technical
{
    public class FPSUnlocker : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}