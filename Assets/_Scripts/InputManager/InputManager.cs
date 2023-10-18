using System;
using UnityEngine;

namespace _Scripts.InputManager
{
    public class InputManager : MonoBehaviour
    {
        public float HorizontalInput { get; private set; }

        private void Update()
        {
            if (Input.GetKeyDown("A"))
            {
                HorizontalInput = -1f;
            }
            else if (Input.GetKeyDown("D"))
            {
                HorizontalInput = 1f;
            }
        }
    }
}