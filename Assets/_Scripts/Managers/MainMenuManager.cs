using System;
using _Scripts.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private Button  _toGarageButton;
        [SerializeField] private Button _toStoreButton;
        [SerializeField] private Button _findGameButton;
        [Inject] private ScenesManager _scenesManager;

        private void Start()
        {
            _toGarageButton.onClick.AddListener(ToGarageBurronPressedHandler);
            _findGameButton.onClick.AddListener(FindGameButtonPressedHandler);
            _toStoreButton.onClick.AddListener(ToStoreButtonPressedHandler);
        }

        private void ToStoreButtonPressedHandler()
        {
            _scenesManager.LoadScene(ScenesManager.SceneType.Store);
        }

        private void FindGameButtonPressedHandler()
        {
            _scenesManager.LoadScene(ScenesManager.SceneType.Connect);
        }

        private void ToGarageBurronPressedHandler()
        {
            _scenesManager.LoadScene(ScenesManager.SceneType.Garage);
        }
    }
}