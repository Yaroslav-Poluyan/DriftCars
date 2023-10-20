using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class ExitGameButton : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(ButtonPressedHandler);
        }

        private void ButtonPressedHandler()
        {
            Application.Quit();
        }
    }
}