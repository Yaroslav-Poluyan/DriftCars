using System;
using UnityEngine;

namespace _Scripts.InputManager
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private TouchPad _touchPad;
        public float GetHorizontalInput()
        {
            return _touchPad.GetHorizontalInput();
        }

        public float GetVerticalInput()
        {
            return _touchPad.GetVerticalInput();
        }
    }
}