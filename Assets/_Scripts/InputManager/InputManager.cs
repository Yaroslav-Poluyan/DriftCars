using System;
using UnityEngine;

namespace _Scripts.InputManager
{
    public class InputManager : MonoBehaviour
    {
        public float HorizontalInput { get; private set; }
        public float VerticalInput { get; private set; }

        private void Update()
        {
            /*HorizontalInput = Input.GetAxis("Horizontal");
            VerticalInput = Input.GetAxis("Vertical");*/
        }
    }
}