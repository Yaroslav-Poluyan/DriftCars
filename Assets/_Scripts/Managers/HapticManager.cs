using System;
using UnityEngine;

namespace _Scripts.Managers
{
    public class HapticManager : MonoBehaviour
    {
        public static readonly bool IsHapticEnabled = true;
        public enum HapticPattern
        {
            OnMudDrive = 0,
            OnDamageTaken = 1,
        }

        public static void PlayHaptic(HapticPattern hapticType)
        {
            if (!IsHapticEnabled) return;
            switch (hapticType)
            {
                case HapticPattern.OnMudDrive:
                    break;
                case HapticPattern.OnDamageTaken:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hapticType), hapticType, null);
            }
        }
    }
}