using UnityEngine;

namespace _Scripts.Managers
{
    public static class Pause
    {
        private static bool _isPaused;

        public static bool IsPaused
        {
            get => _isPaused;
            set
            {
                _isPaused = value;
            }
        }
    }
}