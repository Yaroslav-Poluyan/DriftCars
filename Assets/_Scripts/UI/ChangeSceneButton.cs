using System;
using _Scripts.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _Scripts.UI
{
    public class ChangeSceneButton : MonoBehaviour
    {
        [SerializeField] private ScenesManager.SceneType _sceneType;
        [Inject] private ScenesManager _scenesManager;

        private void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(ChangeScene);
        }

        private void ChangeScene()
        {
            _scenesManager.LoadScene(_sceneType);
        }
    }
}