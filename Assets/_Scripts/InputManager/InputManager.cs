using System;
using UnityEngine;

namespace _Scripts.InputManager
{
    public class InputManager : MonoBehaviour
    {
        public float GetHorizontalInput()
        {
            return Input.GetAxis("Horizontal");
        }

        public float GetVerticalInput()
        {
            return Input.GetAxis("Vertical");
        }
    }
}